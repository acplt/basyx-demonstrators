import asyncio
import configparser
import logging
import traceback
import os

import pycamunda.variable
import pycamunda.externaltask
import urllib.parse
from asyncua import Client
from asyncua import ua
from basyx import aas

from aas_repo_client import AASRepositoryClient

logger = logging.getLogger(__name__)

config = configparser.ConfigParser()
config.read(["config.ini.default", "config.ini"])
LOGLEVEL = config["GENERAL"]["LOGLEVEL"]
MAINLOGLEVEL = config["GENERAL"]["MAINLOGLEVEL"]
CYCLETIME = float(config["GENERAL"]["CYCLETIME"])
WORKER_QUEUE_SIZE = int(config["GENERAL"]["WORKER_QUEUE_SIZE"])
WORKER_COUNT = int(config["GENERAL"]["WORKER_COUNT"])
CAMUNDA_URL = config["CAMUNDA"]["REST_URL"]
CAMUNDA_EXECUTOR_TOPIC = config["CAMUNDA"]["EXECUTOR_TOPIC"]
AAS_URL = config["AAS"]["URL"]
AAS_USER = config["AAS"]["USER"]
AAS_PWD = config["AAS"]["PWD"]
CC_CLIENT_TIMEOUT = float(config["CC"]["CLIENT_TIMEOUT"])
CC_CLIENT_SUBSCRIPTION_MS = int(config["CC"]["CLIENT_SUBSCRIPTION_MS"])
CC_WAITTIME = float(config["CC"]["WAITTIME"])
URL_SCHEME_OPCUA = 'opc.tcp://'

class ExcutorError(Exception):
    pass

class ExternalTaskError(ExcutorError):
    def __init__(self, message, external_task: pycamunda.externaltask.ExternalTask):
        self.externalTask = external_task
        self.message = message + f" while processing '{external_task.activity_id}' in '{external_task.process_definition_key}' for process {external_task.process_instance_id}"
        try:
            locked_external_task_ids.remove(external_task.id_)
        except:
            pass
        super().__init__(self.message)

class ControlComponentError(ExcutorError):
    pass

async def get_activity_instance_submodel_id(external_task: pycamunda.externaltask.ExternalTask):
    variables = pycamunda.variable.GetList(CAMUNDA_URL,
                                           activity_instance_id_in=[external_task.activity_instance_id])()
    for variable in variables:
        if variable.name == "CCSubmodelId":
            submodel_id = variable.value
            # e.g. http://basys4.de/submodels/controlcomponent/instance/demonstrator/NAME
            break
    else:
        raise ExternalTaskError("No CCSubmodelId found", external_task)
    for variable in variables:
        if variable.name == "SkillName":
            skill_name = variable.value
            break
    else:
        raise ExternalTaskError("No SkillName found", external_task)
    return submodel_id, skill_name

async def get_opcua_endpoint_from_cc_submodel(submodel_id: str):
    client = AASRepositoryClient(AAS_URL, AAS_USER)
    client.login(AAS_PWD)
    cc_submodel: aas.model.Submodel = client.get_identifiable(
        aas.model.Identifier(id_=submodel_id, id_type=aas.model.IdentifierType.IRI))
    for endpoint in cc_submodel.get_referable("ControlEndpoints"):
        address = endpoint.get_referable("Address")
        if address.value.startswith(URL_SCHEME_OPCUA):
            opcua_endpoint = address.value
            break
    else:
        raise ExcutorError(f"No OPC UA endpoint found in ControlComponent Submodel: {cc_submodel}")
    return opcua_endpoint    

class ControlComponent():
    def __init__(self, client: Client, browsepath: str):
        self.client = client
        self.cc_path = browsepath.lstrip('/').split('/')
        self.name = self.cc_path[-1].split(':')[-1]
        self.cc_node = None
        self.exst_node = None
        self.opmode_node = None
        self.workst_node = None
        self.operations_node = None
        self.exst = ""
        self.opmode = ""
        self.workst = ""

    class StatusHandler():
        def __init__(self, cc) -> None:
            self.cc = cc
        def datachange_notification(self, node, val, data):
            if node == self.cc.exst_node:
                logger.debug(f"{self.cc.name} received new exst  : {val}")
                self.cc.exst = val
            elif node == self.cc.opmode_node:
                logger.debug(f"{self.cc.name} received new opmode: {val}")
                self.cc.opmode = val
            elif node == self.cc.workst_node:
                logger.debug(f"{self.cc.name} received new workst: {val}")
                self.cc.workst = val       

    async def call(self, method_name:str, namespaceindex = 2):
        logger.debug(f"Calling {method_name} for {self.name} with status: exst={self.exst}, opmode={self.opmode}, workst={self.workst}")
        await self.operations_node.call_method(f"{namespaceindex}:{method_name}")
    
    async def wait(self):
        await asyncio.sleep(CC_WAITTIME)
    
    async def connect(self):
        self.cc_node = await self.client.nodes.root.get_child(self.cc_path)
        logger.debug(f"Resolved {self.name} node id {self.cc_node.nodeid} from {self.cc_path}")
        
        self.exst_node = await self.cc_node.get_child(["2:STATUS", "2:EXST"])
        self.opmode_node = await self.cc_node.get_child(["2:STATUS", "2:OPMODE"])
        self.workst_node = await self.cc_node.get_child(["2:STATUS", "2:WORKST"])
        self.operations_node = await self.cc_node.get_child("2:OPERATIONS")
        
        sub = await self.client.create_subscription(CC_CLIENT_SUBSCRIPTION_MS, ControlComponent.StatusHandler(self))
        #TODO save handle to subsciptions (check if no Status Code is bad) and cance when cc is deleted
        await sub.subscribe_data_change([self.exst_node, self.opmode_node, self.workst_node])
        logger.debug(f"Connected to {self.name} with node ids: exst={self.exst_node.nodeid}, opmode={self.opmode_node.nodeid}, workst={self.workst_node}, operations={self.operations_node.nodeid}")
    
    async def reset(self):
        if self.exst == "IDLE":
            return
        while self.exst not in ["STOPPED", "COMPLETE", "IDLE"]:
            await self.wait()
        if self.exst == "IDLE":
            return
        await self.call("RESET")
        while self.exst in ["STOPPED", "COMPLETE", "RESETTING"]:
            await self.wait()
        if self.exst in ["ABORTING", "ABORTED", "STOPPING", "STOPPED", "CLEARING"]:
            raise ControlComponentError(f"Stopped or aborted while resetting {self.name} with exst {self.exst}")
        if self.exst != "IDLE":
            raise ControlComponentError(f"Not in idle after resetting {self.name} with exst {self.exst}")

    async def select(self, opmode):
        if self.opmode == opmode:
            return
        await self.call(opmode, 3)
        while self.opmode != opmode:
            await self.wait()
    
    async def start(self):
        await self.call("START")
        while self.exst == "IDLE":
            await self.wait()
        if self.exst in ["ABORTING", "ABORTED", "STOPPING", "STOPPED", "CLEARING", "RESETTING"]:
            raise ControlComponentError(f"Stopped or aborted while starting {self.opmode} for {self.name} with exst {self.exst}")
    
    async def wait_for_completed(self, opmode):
        while True:
            if self.exst == "COMPLETE" and self.opmode == opmode:
                break
            await self.wait()
        if self.exst in ["ABORTING", "ABORTED", "STOPPING", "STOPPED", "CLEARING", "RESETTING"]:
            raise ControlComponentError(f"Stopped or aborted while executing {self.opmode} for {self.name} with exst {self.exst}")

async def connect_and_execute_op_mode(opcua_endpoint: str, skill_name: str):
    # e.g. opc.tcp://localhost:4861/0:Objects/1:ESE/1:RB01
    opcua_url = urllib.parse.urlparse(opcua_endpoint)

    async with Client(url=f"{opcua_url.scheme}://{opcua_url.netloc}",timeout = CC_CLIENT_TIMEOUT) as client:
        cc = ControlComponent(client, opcua_url.path)
        await cc.connect()
        
        # Wait for IDLE and reset if necessary
        await cc.reset()
        # Select opmode (and reset if necessery)
        await cc.select(skill_name)
        await cc.reset()
        # Send start and Wait for not IDLE / STARTING, break by ABORTED, STOPPED cycles
        await cc.start()
        # Wait for COMPLETE and OPMODE == skill_name, break by ABORTED, STOPPED cycles
        await cc.wait_for_completed(skill_name)

_CAMUNDA_WORKER_ID = os.path.basename(__file__)+ "-" + str(os.getpid())
locked_external_task_ids = []

async def worker(name, queue):
    while True:
        external_task = await queue.get()
        logger.info(f"{name} processing '{external_task.activity_id}' in '{external_task.process_definition_key}' for process {external_task.process_instance_id}")
        try:
            submodel_id, skill_name = await get_activity_instance_submodel_id(external_task)
            logger.debug(f"{external_task.activity_id}: Extracted skill name '{skill_name}' from external task with submodel id: {submodel_id}")

            # looks up the OPC UA endpoint of the control component
            # which was requested by the service task in the AAS repository server via its HTTP/REST API
            opcua_endpoint = await get_opcua_endpoint_from_cc_submodel(submodel_id)
            logger.debug(f"{external_task.activity_id}: Extracted opcua endpoint '{opcua_endpoint}' from external task.")

            # connects to the OPC UA endpoint and executes the desired operation mode (skill), via a control component protocol.
            await connect_and_execute_op_mode(opcua_endpoint, skill_name)
        
            logger.info(f"{name} completed '{external_task.activity_id}' in '{external_task.process_definition_key}' for process {external_task.process_instance_id}")
            locked_external_task_ids.remove(external_task.id_)
            pycamunda.externaltask.Complete(CAMUNDA_URL, external_task.id_, _CAMUNDA_WORKER_ID)()

        except Exception as e:
            logger.warning(f"{e} while processing external task: {str(external_task)}")
            locked_external_task_ids.remove(external_task.id_)
            try:
                pycamunda.externaltask.HandleFailure(CAMUNDA_URL, external_task.id_, _CAMUNDA_WORKER_ID, str(e), f"Error while executing skill {skill_name} on endpoint {opcua_endpoint} with submodel id {submodel_id}", 0, 500)()        
            except:
                pass
        queue.task_done()

async def main():
    queue = asyncio.Queue(WORKER_QUEUE_SIZE)
    
    tasks = []
    for i in range(WORKER_COUNT):
        task = asyncio.create_task(worker(f'Worker-{i}', queue))
        tasks.append(task)
    
    logger.info(f"Starting main loop with {CYCLETIME}s cycletime and {WORKER_COUNT} tasks.")
    while True:
        await asyncio.sleep(CYCLETIME)
        
        for external_task_id in locked_external_task_ids:
            try:
                pycamunda.externaltask.ExtendLock(CAMUNDA_URL, external_task_id, 2000*CYCLETIME, _CAMUNDA_WORKER_ID)()
            except Exception as e:
                locked_external_task_ids.remove(external_task_id)
                logger.warn(str(e) + ". Couldn't extend lock for fetched external tasks: "+ str(locked_external_task_ids))
        
        if queue.full():
            continue
        try:
            # queries the workflow engine Camunda for open service tasks via the Camunda HTTP/REST API
            fetch_tasks = pycamunda.externaltask.FetchAndLock(CAMUNDA_URL, _CAMUNDA_WORKER_ID, queue.maxsize-queue.qsize())
            fetch_tasks.add_topic(CAMUNDA_EXECUTOR_TOPIC, 2000*CYCLETIME)
            for external_task in fetch_tasks():
                queue.put_nowait(external_task)
                locked_external_task_ids.append(external_task.id_)
        except Exception as e:
            logger.warn(str(e) + ". Couldn't receive external task list.")


if __name__ == '__main__':
    logging.basicConfig(level=LOGLEVEL) #TODO restrict to logging._nameToLevel.keys()
    logger.setLevel(MAINLOGLEVEL)
    logger.debug("Running with configuration: {}".format({s: dict(config.items(s)) for s in config.sections()}))    
    try:
        asyncio.run(main())
    except Exception as e:
        print(traceback.format_exc())
        logger.error(e)
