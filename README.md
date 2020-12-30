
# TestContainersExample
 A .NET Core example using Testcontainers.net (unofficial version from HofmeisterAn), evolve and stuff.

## Status
![TC-Example-Build](https://github.com/Pinguwien/TestContainersExample/workflows/TC-Example-Build/badge.svg)

## What it does
This demo shows some basic concepts of [Testcontainers](https://github.com/HofmeisterAn/dotnet-testcontainers/) by spinning up a postgres container in the tests to do some integration testing using a postgres db. 

## Prerequisites
* Docker and .NET SDK (or access to GitHub and GitHub Actions)

## Used Libraries and Frameworks:
* .NET Core 5.0.101
* Postgres Docker image (postgres:alpine-13.1)
* Evolve 2.4.0 (DB Migrations)
* Npgsql 5.0.1.1 (postgres connection)
* NUnit 3.12.0 (testing)
* DotNet.Testcontainers 1.5.0-beta.20201201.11 (testing, duh!)
* Dapper 2.0.78 (showing basic crud principles)

## Roadmap
For now, this repository is fine to show the basic concepts. But there's much more possible and may eventually land here: 

* Use other DBMS (NoSQL, whatever)
* Well, Keycloak would be nice to test against. 
* Also, a MQ example would be nice. 