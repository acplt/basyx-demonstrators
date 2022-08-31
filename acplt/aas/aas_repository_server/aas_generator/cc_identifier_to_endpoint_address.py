from typing import List
import configparser
import os

from basyx.aas import model

from aas_generator import control_component_submodel
import storage

config = configparser.ConfigParser()
config.read(
    ["aas_generator/config.ini.default",
     "aas_generator/config.ini"])

BASE_SM_URL: str = str(config["SUBMODEL"]["BASE_SM_URL"])
BASE_OPC_URL: str = str(config["SUBMODEL"]["BASE_OPC_URL"])
ASSET_IDS: List[str] = str(config["ASSETS"]["IDS"]).splitlines()
STORAGE_DIR = str(config["SUBMODEL"]["STORAGE_DIR"])

cc_submodels: List[model.Submodel] = []
for i in ASSET_IDS:
    print(f"Generate CC Submodel in memory for {i}\n\tidentifier_iri = {BASE_SM_URL.format(i)}\n\tendpoint_address = {BASE_OPC_URL.format(i)}")
    cc_submodels.append(
        control_component_submodel.generate_control_component_submodel(
            identifier_iri=BASE_SM_URL.format(i),
            endpoint_address=BASE_OPC_URL.format(i)
        )
    )

def write_cc_submodels_to_aas_repository_server_store():
    if not os.path.exists(STORAGE_DIR):
        print("Create new storage dir at: " + os.path.realpath(STORAGE_DIR))
        os.makedirs(STORAGE_DIR)
    store = storage.RegistryObjectStore(STORAGE_DIR)
    store.clear()
    for s in cc_submodels:
        print("Add CC Submodel to Repository: {}".format(s))
        store.add(s)

if __name__ == '__main__':
    print("Running with configuration: {}".format({s: dict(config.items(s)) for s in config.sections()}))
    write_cc_submodels_to_aas_repository_server_store()
