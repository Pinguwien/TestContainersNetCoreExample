
# TestContainersExample
 A .NET Core example using Testcontainers.net (unofficial version from HofmeisterAn), evolve and stuff.

## Status
[![Actions Status](https://github.com/pinguwien/{repo}/workflows/{workflow_name}/badge.svg)](https://github.com/{owner}/{repo}/actions)

## What it does
This demo shows the basic concepts of testcontainers by spinning up a postgres container in the tests to do some integration testing using a postgres db 

## Prerequisites
* Docker installed (or access to GitHub and GitHub Actions)

## Used Libraries and Frameworks:
* .NET Core 5.0.101
* Postgres Docker image (postgres:alpine-13.1)
* Evolve 2.4.0 (DB Migrations)
* Npgsql 5.0.1.1 (postgres connection)
* NUnit 3.12.0 (testing)
* DotNet.Testcontainers 1.5.0-beta.20201201.11 (testing right)