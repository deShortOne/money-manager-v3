# Money Tracker V3

A budget tracker for tracking your bills and how much you've spent in different categories

## Purpose of this repo
This repo is overengineered as heck but this is my fun repo to implement a bunch of patterns

CQRS as the architecture (currently it's a distributed monolith, but there are plans to convert it into a true microservice).
Controller-Service-Repository as the architecture to keep logic separated.
Chain of responsibility in wage calculator. This pattern was used because not all factors are needed for all people and by separating out the classes, it makes it easier to test and to mix and match. 

## How to run
### Backend
#### Running Command and Query together using docker
You will need to setup aws using the below command. Which permissions
```bash
aws configure --profile "money-tracker-dotnet"
```

Run in the root directory
```bash
docker compose up
```
#### Running Command and Query separately using docker
For both run this first, as both depends on this image and it only needs to be run once
```bash
docker build --rm -f ./Dockerfile-Base -t be-base-image:latest .
```
Ensure you replace ${...} with the proper values. There may be other values you will have to replace.

For Command, run these commands
```bash
docker build -f ./Commands/Dockerfile -t be-commands-image:latest .

docker run -e Database:Paelagus_RO="User ID=${username};Password=${password};Host=172.17.0.1;Port=5432;Database=${database}" -e ASPNETCORE_ENVIRONMENT="Development" -p 1235:8080 be-commands-image
```

For Query, run these commands
```bash
docker build -f ./Queries/Dockerfile -t be-queries-image:latest .

docker run -e Database:Paelagus_RO="User ID=${username};Password=asdf;Host=172.17.0.1;Port=5432;Database=deshortone" -e ASPNETCORE_ENVIRONMENT="Development" -p 1235:8080 be-queries-image
```
## How to test

### In the cloud
Simply run in github actions

### Docker engine (in WSL) on windows
Ensure dockerd is running
Then run dotnet test, run tests in an ide or run in [act](https://github.com/nektos/act).
This is how I run my tests so here it is first

#### Tests not running due to connection being refused?
In ubuntu, create a file
```
/etc/docker/daemon.json
```
And put the following in it
```json
{
    "hosts": ["tcp://0.0.0.0:2375","unix:///var/run/docker.sock"]
} 
```

### Docker desktop on windows
In each of the test .csproj, remove the property group so that RUN_LOCAL is not set

## Code coverage
Run this in root directory
```bash
rm -r coverage --force \
&& dotnet test --verbosity q --collect:"XPlat Code Coverage" --results-directory coverage \
&& dotnet-coverage merge coverage/**/coverage.cobertura.xml -f cobertura -o coverage/coverage.xml \
&& reportgenerator -reports:"coverage/coverage.xml" -targetdir:"coverage/coveragereport" -reporttypes:Html \
&& firefox ./coverage/coveragereport/index.htm
```
