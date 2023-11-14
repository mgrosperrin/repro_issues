# Multi/combined consent

## Context
ProductApplication & MozzAPI are two API protected by Entra ID authentication.

ProductApplication calls MozzAPI that will calls MS Graph to retrieve the display name of the user's tenant.

Consenting to ProductApplication should automatically consent to MozzAPI.


## Reproduction

`mozzapi_app-registration.json` and `productapplication_app-registration.json` contains the manifest of the two app registration. Be carrefull with the ID, they have to be adapted to your own context!