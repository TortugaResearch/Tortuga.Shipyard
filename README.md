# Tortuga Shipyard

A library for generating datbase schema. 


## Mission Statement

The goal of this project is to make it easier to code-generate database schema. It's not meant to be a complete solution. Rather, it is designed to replace string-concatenation in your own code generators.

The initial use case is Tortuga Chain's testing framework. The goal is to be able to define an abstract table once and allow Shipyard to produce the SQL necessary for each database engine that Chain supports.

Additional functionality such as generating views and stored procedures will be considered.

This is not intended to be a replacement for database management tools such as SQL Server Data Tools or Liquibase. Rather, it can be used as an input for those tools. 

