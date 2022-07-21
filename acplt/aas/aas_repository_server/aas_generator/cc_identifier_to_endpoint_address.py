from typing import Dict, List

from basyx.aas import model

import control_component_submodel
from aas_repository_server import storage

_BASE_SM_URL: str = "http://basys4.de/submodels/controlcomponent/instance/demonstrator/"
_BASE_OPC_URL: str = "opc.tcp://localhost:4840/0:Objects/1:CCs/1:"

ASSET_IDs: List[str] = [
    "BP11",
    "GF01",
    "GF02",
    "GF03",
    "GF04",
    "HA02",
    "KR01",
    "PL01",
    "PM01",
    "PP01",
    "PP02",
    "PP04",
    "PS01",
    "RB01",
    "RB02",
    "SP01",
    "TS01",
    "VA02",
]


"""
A simple Dict, mapping CC Submodel Identifier IRIs to OPC-UA Endpoint Addresses
"""
CC_IDENTIFIER_TO_ENDPOINT_ADDRESS: Dict[str, str] = {}
for i in ASSET_IDs:
    CC_IDENTIFIER_TO_ENDPOINT_ADDRESS["{}{}".format(_BASE_SM_URL, i)] = "{}{}".format(_BASE_OPC_URL, i)

cc_submodels: List[model.Submodel] = []
for identifier_iri, opc_endpoint in CC_IDENTIFIER_TO_ENDPOINT_ADDRESS.items():
    print("Generate CC Submodel for {}".format(identifier_iri))
    cc_submodels.append(
        control_component_submodel.generate_control_component_submodel(
            identifier_iri=identifier_iri,
            endpoint_address=opc_endpoint
        )
    )


def write_cc_submodels_to_aas_repository_server_store():
    store = storage.RegistryObjectStore("../store")
    store.clear()
    for s in cc_submodels:
        print("Add CC Submodel to Repository: {}".format(s))
        store.add(s)


if __name__ == '__main__':
    write_cc_submodels_to_aas_repository_server_store()
