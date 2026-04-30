# Stockaholic
Stockaholic is a basic stock management application developed as a school project. It allows users to keep track of inventory, manage stock levels, and organize products in a simple and efficient way. The goal of this project is to demonstrate fundamental concepts of application development, including data handling and API design.

## Running
This project contains Docker and Docker Compose configuration to run the API and a Postgres database.

- `docker-compose.dev.yml` — development compose: starts a Postgres instance (DB only).
- `docker-compose.prod.yml` — production compose: builds/runs the API and Postgres together.
### Prerequisites
- Docker (Engine) and Docker Compose. On modern Linux distributions Docker Compose is included with Docker Engine.
### Dev mode
```bash
docker compose -f docker-compose.dev.yml up -d
```
```bash
dotnet run --project Stockaholic.Api/Stockaholic.Api.csproj
```
```bash
dotnet run --project Stockaholic.Frontend/Stockaholic.Frontend.csproj
```
Open http://localhost:8080/swagger - for the api documentation

Open http://localhost:5018 - for the frontend

Now you're ready to start stocking

### Prod mode
```bash
docker compose -f docker-compose.prod.yml up -d
```

Open http://localhost:5018 - for the frontend

Now you're ready to start stocking