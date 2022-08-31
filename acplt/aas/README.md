# Asset Administration Shell Repository Server

This folder contains a simple AAS repository server based on the 
[BaSyx Python SDK](https://github.com/eclipse-basyx/basyx-python-sdk).

The server runs on `http://localhost:1120`, for details see `aas_repository_server/config.ini`.
(You can remember the Port via `AAS`: `A`: 1st letter of the alphabet, `S`: 20th letter of the alphabet)

*Note:* Port 1120 is apparently used by the battle.net file transfer protocol. We advise against running this 
demonstrator, if you are playing World of Warcraft at the same time.

**Hacky-Hacky Note**
This server provides the most basic functionalities and is **not** maintained. 
The maintained version will be found in an adequately named repository at https://github.com/acplt/ 
as soon as some organisatorical problems are solved. 
There, also more and better documentation will be found.

**IT IS NOT SAFE TO USE THIS SERVER IN PRODUCTION!**

## Installation Instructions

This assumes, you have Python with a version >= 3.9 installed
```shell
pip install -r requirements.txt
```

## How to use

### Adding a User (The Hacky Way)

Adding a user is simple, when using the authentication CLI:

```shell
python auth.py
```

It is advised to remember the password, since it is hashed and salted and can not be retrieved.


### Adding AAS objects to the repository (The Hacky Way)

1. Read the AAS in to the basyx-python SDK
2. Import the `storage.RegistryObjectStore` 
3. Add them to the RegistryObjectStore via the `.add()` function

For the ease of use, run from `aas_repository_server` directory: 
```shell
python -m aas_generator.cc_identifier_to_endpoint_address
```
This automatically clears, generates and repopulates the AAS Repository Server with the suiting Control Component Submodels needed for this Demonstrator configured by `aas_generator/config.ini`, for syntax and defautl values check `aas_generator/config.ini.default`

### Running the Server (The Hacky Way)

Write a `config.ini`, for syntax and default values check `config.ini.default`

```shell
python routes.py
```
