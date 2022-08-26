import traceback
try:
    import datetime
    import os
    import configparser
    import argparse
    import json
    from typing import Optional, Set, Dict

    import flask
    import jwt
    import werkzeug.security

    from basyx.aas import model
    from basyx.aas.adapter.json import json_serialization, json_deserialization
    from aas_repository_server import auth, storage
    from aas_repository_server.aas_generator import cc_identifier_to_endpoint_address

    APP = flask.Flask(__name__)
    config = configparser.ConfigParser()
    config.read(["config.ini.default"])
    config.read(["config.ini"])

    # Read config file
    JWT_EXPIRATION_TIME: int = int(config["AUTHENTICATION"]["TOKEN_EXPIRATION_TIME"])  # JWT Expiration Time in minutes
    PORT: int = int(config["GENERAL"]["PORT"])
    HOST: str = str(config["GENERAL"]["HOST"])
    STORAGE_DIR: str = os.path.abspath(config["STORAGE"]["STORAGE_DIR"])
    OBJECT_STORE: storage.RegistryObjectStore = storage.RegistryObjectStore(STORAGE_DIR)
    # todo: Create storage dir, if not existing

    CC_PORT: int = int(config["CONTROLCOMPONENT"]["PORT"])
    CC_HOST: str = str(config["CONTROLCOMPONENT"]["HOST"])

    @APP.route("/login", methods=["GET", "POST"])
    def login_user():
        """
        Login a user with basic authentication and respond with a new JWT, if the authentication was successful.
        """
        if not flask.request.authorization \
                or not flask.request.authorization.username \
                or not flask.request.authorization.password:
            return flask.make_response("Unauthorized", 401)
        username: str = flask.request.authorization.username
        if not auth.check_if_user_exists(username):
            print("Unknown user '{}'".format(username))
            return flask.make_response("Invalid User or Password", 401)
        if werkzeug.security.check_password_hash(auth.get_password_hash(username), flask.request.authorization.password):
            token = jwt.encode(
                {
                    'name': username,
                    'exp': datetime.datetime.utcnow() + datetime.timedelta(minutes=JWT_EXPIRATION_TIME)
                },
                auth.SECRET_KEY,
                algorithm="HS256"
            )
            print("User '{}' successful login".format(username))
            return flask.json.dumps({"token": token})
        else:
            print("User '{}' invalid password".format(username))
            return flask.make_response("Invalid User or Password", 401)


    @APP.route("/test_connection", methods=["GET"])
    def test_connection():
        """
        Returns "success", if everything is fine
        """
        return flask.make_response("Success", 200)


    @APP.route("/test_authorized", methods=["GET"])
    @auth.token_required
    def test_authorized(current_user: str):
        """
        Tests if the connection can be established and the user is authorized

        :return:
        """
        return flask.json.dumps({"Connection": "ok", "User": current_user}), 200


    @APP.route("/get_identifiable", methods=["GET"])
    @auth.token_required
    def get_identifiable(current_user: str):
        """
        Executes an SQL statement with an MySQL user, that only has `SELECT`, `CREATE VIEW` and `INDEX` and `SHOW VIEW`

        Request format is a json serialized :class:`basyx.aas.model.base.Identifier`:

        .. code-block::

            {
                "id": "<Identifier.id string>",
                "idType": "<idType string>"
            }

        Returns a JSON serialized :class:`basyx.aas.model.base.Identifiable`.

        :returns:

            - 200, with the Identifiable
            - 400, if the request cannot be parsed
            - 404, if no result is found
            - 422, if a valid AAS object was given, but not an Identifiable
         """
        data = flask.request.get_data(as_text=True)
        # Load the JSON from the request
        try:
            identifier_dict: Dict[str, str] = json.loads(data)
        except json.decoder.JSONDecodeError:
            return flask.make_response("Could not parse request, not valid JSON", 400)
        # Check that the request JSON contained in fact an Identifier
        try:
            identifier: model.Identifier = model.Identifier(
                id_=identifier_dict["id"],
                id_type=json_deserialization.IDENTIFIER_TYPES_INVERSE[identifier_dict["idType"]]
            )
        except KeyError:
            return flask.make_response("Request does not contain an Identifier", 422)
        # Try to resolve the Identifier in the object store
        identifiable: Optional[model.Identifiable] = OBJECT_STORE.get(identifier)
        # Todo: Check here if the given user has access rights to the Identifiable
        if identifiable is None:
            return flask.make_response("Could not find Identifiable with id {} in repository".format(identifier.id), 404)
        return flask.make_response(
            json.dumps(identifiable, cls=json_serialization.AASToJsonEncoder, indent=4),
            200
        )


    @APP.route("/query_semantic_id", methods=["GET"])
    @auth.token_required
    def query_semantic_id(current_user: str):
        """
        Query all Identifiable objects that either have a semanticID or contain a child having that semanticID.

        Request format is a json serialized :class:`basyx.aas.model.base.Key` (from a Reference):

        .. code-block::

            {
                'type': 'GlobalReference',
                'idType': 'IRI',
                'value': 'https://example.com/semanticIDs/ONE',
                'local': False
            }

        Returns a list of identifiers (in no particular order) that contain that semanticID

        :returns:

            - 200, with the List of Identifiers
            - 400, if the request cannot be parsed
            - 422, if a valid AAS object was given, but not an Identifiable
        """
        data = flask.request.get_data(as_text=True)
        # Load the JSON from the request
        try:
            key_dict: Dict[str, str] = json.loads(data)
        except json.decoder.JSONDecodeError:
            return flask.make_response("Could not parse request, not valid JSON", 400)
        # Check that the request JSON contained in fact an Identifier
        try:
            semantic_id_key: model.Key = model.Key(
                type_=json_deserialization.KEY_ELEMENTS_INVERSE[key_dict["type"]],
                local=True if key_dict["local"] else False,
                value=key_dict["value"],
                id_type=json_deserialization.KEY_TYPES_INVERSE[key_dict["idType"]]
            )
        except KeyError:
            return flask.make_response("Request does not contain a Key", 422)
        # Get the list of identifiables that contain the semanticID
        identifiers: Optional[Set[model.Identifier]] = OBJECT_STORE.semantic_id_index.get(semantic_id_key)
        # Todo: Check here if the given user has access rights to the Identifiable
        if identifiers is None:
            identifiables = set([])  # Set the identifiables to an empty set
        return flask.make_response(
            json.dumps(list(identifiers), cls=json_serialization.AASToJsonEncoder, indent=4),
            200
        )


except Exception as e:
    tb = traceback.format_exc()
    print(tb)
    print(e)
    input("Ups")

if __name__ == '__main__':
    cli_parser = argparse.ArgumentParser(description='Runs an Asset Administration Shell server via flask')
    cli_parser.add_argument('-u', '--update', action='store_true', help='update the storage by generating submodels for control components (also updates their OPC UA endpoints from config)')
    args = cli_parser.parse_args()

    print("Running with configuration: {}".format({s: dict(config.items(s)) for s in config.sections()}))
    print("Found {} Users".format(len(auth.USERS)))
      
    if args.update:
        cc_identifier_to_endpoint_address.update_cc_submodel_endpoints(CC_HOST, CC_PORT)
        cc_identifier_to_endpoint_address.write_cc_submodels_to_aas_repository_server_store(STORAGE_DIR)
    
    APP.run(port=PORT, host=HOST)
