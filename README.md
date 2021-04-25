
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
* Evolve 3.0.0 (DB Migrations)
* Npgsql 5.0.1.1 (postgres connection)
* NUnit 3.12.0 (testing)
* DotNet.Testcontainers 1.5.0-beta.20201201.11 (testing, duh!)
* Dapper 2.0.78 (showing basic crud principles)
* GitHub Actions for CI/CD

## TODO
For now, this repository is fine to show the basic concepts. But there's much more possible and may eventually land here. Any help is very appreciated, feel free to drop me a line:

* Use other DBs (NoSQL, whatever)
* Add Swashbuckle
* Use Keycloak.X
* perhaps clean up some tests which are only for showing some concepts on master to reduce CI-time (nearly 2 minutes at the time of writing)
* A Message Queuing tc example (e.g. rabbitmq)
* apply tactical DDD-Concepts, e.g. The "Article" in "Domain" is too tightly coupled to the db layer due to its ID param which is generated by the DBMS. Also, use Value Objects (perhaps fiddle around with records?)
* ...? 

## Done
This is just to put the TODO items which were integrated somewhere ;)

* First of all, add a real API in front of the repo. I mostly walked through the Test-Solution here, the app might get an OpenAPI Frontend, perhaps access control, and things like that. tl;dr: Make "DemoApp" a real app :-)
* Keycloak/IS4 integration via testcontainers would be nice to test against. (May be a better Solution to use testcontainers than [FakeAuth](https://github.com/Pinguwien/DotNetCoreFakeAuth)) 
