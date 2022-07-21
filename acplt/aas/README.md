# Asset Administration Shell Repository Server

This folder contains a simple AAS repository server based on the 
[BaSyx Python SDK](https://github.com/eclipse-basyx/basyx-python-sdk).

**Hacky Hacky Notice**
This server provides the most basic functionalities and is **not** maintained. 
The maintained version will be found in a adequately named repository at https://github.com/acplt/ 
as soon as some organisatorical problems are solved.

## Installation Instructions

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

### Running the Server (The Hacky Way)

Write a `config.ini`, for syntax check `config.ini.default`

```shell
python routes.py
```
