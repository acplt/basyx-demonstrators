import asyncio
import configparser
import traceback

import pycamunda.variable
import pycamunda.externaltask
from asyncua import Client
from basyx import aas
from basyx.aas.model import Identifiable

from acplt.executor.aas_repo_client import AASRepositoryClient

config = configparser.ConfigParser()
config.read(["config.ini"])
CAMUNDA_URL = config["GENERAL"]["CAMUNDA_REST_URL"]
AAS_REPO_URL = config["GENERAL"]["AAS_REPO_URL"]
AAS_REPO_USER = config["GENERAL"]["AAS_REPO_USER"]
AAS_REPO_PWD = config["GENERAL"]["AAS_REPO_PWD"]
OPCUA_URL_PREFIX = 'opc.tcp://'


async def get_activity_instance_submodel_id(external_task: pycamunda.externaltask.ExternalTask):
    variables = pycamunda.variable.GetList(CAMUNDA_URL,
                                           activity_instance_id_in=[external_task.activity_instance_id])()
    for variable in variables:
        if variable.name == "submodelId":
            submodel_id = variable.name
            # e.g. http://basys4.de/submodels/controlcomponent/instance/demonstrator/NAME
            break
    else:
        raise ValueError(f"No SubmodelId found")
    return submodel_id


async def get_opcua_endpoint_from_cc_submodel(submodel_id: str):
    client = AASRepositoryClient(AAS_REPO_URL, AAS_REPO_USER)
    client.login(AAS_REPO_PWD)
    cc_submodel: aas.model.Submodel = client.get_identifiable(
        aas.model.Identifier(id_=submodel_id, id_type=aas.model.IdentifierType.IRI))
    for endpoint in cc_submodel.get_referable("ControlEndpoints"):
        address = endpoint.get_referable("Address")
        if address.value.startswith(OPCUA_URL_PREFIX):
            opcua_endpoint = address.value
            break
    else:
        raise ValueError(f"No OPCUA endpoint found in ControlComponent Submodel: {cc_submodel}")
    return opcua_endpoint


async def connect_and_execute_op_mode(opcua_endpoint: str):
    # e.g. opc.tcp://localhost:4861/0:Objects/1:ESE/1:RB01
    endpoint_parts = opcua_endpoint.lstrip(OPCUA_URL_PREFIX).split("/")
    opcua_url = f"{OPCUA_URL_PREFIX}{endpoint_parts[0]}"
    node_path = endpoint_parts[1:]

    async with Client(url=opcua_url) as client:
        print(f"Root node is: {client.nodes.root}")
        print(f"Objects node is: {client.nodes.objects}")
        print(f"Children of root are: {await client.nodes.root.get_children()}")

        obj = await client.nodes.root.get_child(node_path)
        children = obj.get_children()
        # TODO: browse the obj and find desired variables and funcs


async def main():
    # queries the workflow engine Camunda for open service tasks with a specific tag via the Camunda HTTP/REST API
    external_tasks = pycamunda.externaltask.GetList(CAMUNDA_URL)()
    # run x(0)..x(10) concurrently and process results as they arrive
    for external_task in asyncio.as_completed(external_tasks):
        submodel_id = await get_activity_instance_submodel_id(external_task)

        # looks up the OPC UA endpoint of the control component
        # which was requested by the service task in the AAS repository server via its HTTP/REST API
        opcua_endpoint = await get_opcua_endpoint_from_cc_submodel(submodel_id)

        # connects to the OPC UA endpoint and executes the desired operation mode (skill), via a control component protocol.
        await connect_and_execute_op_mode(opcua_endpoint)


if __name__ == '__main__':
    try:
        asyncio.run(main())
        # TODO: check if async funktioniert
    except Exception as e:
        print("Running with configuration: {}".format({s: dict(config.items(s)) for s in config.sections()}))
        tb = traceback.format_exc()
        print(tb)
        print(e)
        input("Ups")
