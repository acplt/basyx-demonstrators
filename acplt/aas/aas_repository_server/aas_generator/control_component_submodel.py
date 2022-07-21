from typing import List

from basyx.aas import model


CONTROL_COMPONENT_INSTANCE_SUBMODEL_SEMANTIC_ID: model.Reference = model.Reference(tuple([model.Key(
    type_=model.KeyElements.GLOBAL_REFERENCE,
    local=False,
    value="https://wiki.eclipse.org/BaSyx_/_Submodels#Control_Component_Instance",
    id_type=model.KeyType.IRI
)]))
CONTROL_ENDPOINT_SMC_SEMANTIC_ID: model.Reference = model.Reference(tuple([model.Key(
    type_=model.KeyElements.GLOBAL_REFERENCE,
    local=False,
    value="https://wiki.eclipse.org/BaSyx_/_Documentation_/_ControlComponentProfiles#Interface_Specification",
    id_type=model.KeyType.IRI
)]))
CONTROL_ENDPOINT_SMC_ADDRESS_PROPERTY_SEMANTIC_ID = model.Reference(tuple([model.Key(
    type_=model.KeyElements.GLOBAL_REFERENCE,
    local=False,
    value="https://wiki.eclipse.org/BaSyx_/_Documentation_/_ControlComponentProfiles#Address",
    id_type=model.KeyType.IRI
)]))
CONTROL_ENDPOINT_SMC_PROFILE_PROPERTY_SEMANTIC_ID = model.Reference(tuple([model.Key(
    type_=model.KeyElements.GLOBAL_REFERENCE,
    local=False,
    value="https://wiki.eclipse.org/BaSyx_/_Documentation_/_API_/_ControlComponentProfiles#Profile_Definition",
    id_type=model.KeyType.IRI
)]))


class ControlEndpointSubmodelElementCollection(model.SubmodelElementCollectionUnordered):
    def __init__(self,
                 id_short: str,
                 address: str,
                 profile: str = "ACPLT-DEMO"):
        super().__init__(id_short, semantic_id=CONTROL_COMPONENT_INSTANCE_SUBMODEL_SEMANTIC_ID)
        self.address_property: model.Property = model.Property(
            id_short="Address",
            value_type=model.datatypes.String,
            value=address,
            category="PARAMETER",
            semantic_id=CONTROL_ENDPOINT_SMC_ADDRESS_PROPERTY_SEMANTIC_ID
        )
        self.profile_property: model.Property = model.Property(
            id_short="Profile",
            value_type=model.datatypes.String,
            value=profile,
            category="CONSTANT",
            semantic_id=CONTROL_ENDPOINT_SMC_PROFILE_PROPERTY_SEMANTIC_ID
        )
        self.value.add(self.address_property)
        self.value.add(self.profile_property)


class OperationModeSubmodelElementCollection(model.SubmodelElementCollectionUnordered):
    def __init__(self, id_short: str, operation_name: str):
        super().__init__(id_short=id_short)
        self.operation_name_property: model.Property = model.Property(
            id_short="OperationName",
            value_type=model.datatypes.String,
            value=operation_name
        )
        self.parameters_smc = model.SubmodelElementCollectionUnordered(id_short="Parameters")
        self.value.add(self.operation_name_property)
        self.value.add(self.parameters_smc)


class ControlComponentInstanceSubmodel(model.Submodel):
    def __init__(self,
                 identification: model.Identifier,
                 control_endpoints: List[ControlEndpointSubmodelElementCollection],
                 operation_modes: List[OperationModeSubmodelElementCollection]):
        super().__init__(
            identification,
            semantic_id=CONTROL_COMPONENT_INSTANCE_SUBMODEL_SEMANTIC_ID,
            id_short="ControlComponentInstance",
            kind=model.ModelingKind.INSTANCE
        )
        self.control_endpoints_smc = model.SubmodelElementCollectionUnordered(id_short="ControlEndpoints")
        self.operation_modes_smc = model.SubmodelElementCollectionUnordered(id_short="OperationModes")
        self.submodel_element.add(self.control_endpoints_smc)
        self.submodel_element.add(self.operation_modes_smc)
        for control_endpoint in control_endpoints:
            self.control_endpoints_smc.value.add(control_endpoint)
        for operation_mode in operation_modes:
            self.operation_modes_smc.value.add(operation_mode)


def generate_control_component_submodel(
        identifier_iri: str,
        endpoint_address: str,
) -> ControlComponentInstanceSubmodel:
    cc_endpoint_smc = ControlEndpointSubmodelElementCollection(
        id_short="ControlEndpoint01",
        address=endpoint_address
    )
    operation_mode_smc = OperationModeSubmodelElementCollection(
        id_short="OperationMode01",
        operation_name="Duck"
    )
    return ControlComponentInstanceSubmodel(
        identification=model.Identifier(id_=identifier_iri, id_type=model.IdentifierType.IRI),
        control_endpoints=[cc_endpoint_smc],
        operation_modes=[operation_mode_smc]
    )
