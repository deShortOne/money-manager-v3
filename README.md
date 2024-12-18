# Money Tracker V3

A budget tracker for tracking your bills and how much you've spent in different categories

## How to run
### Backend
#### Running Command and Query together using docker
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
Either use [act](https://github.com/nektos/act), run dotnet test or run in visual studio(but ensure dockerd is running)
Will need docker

### Tests not running due to connection being refused?
Might need to enable dockerd
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
The run
```bash
sudo dockerd
```

## Code coverage
Run this in root directory
```bash
rm -rf coverage/ && dotnet build && dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory coverage && dotnet-coverage merge coverage/**/coverage.cobertura.xml -f cobertura -o coverage/coverage.xml && pycobertura show coverage/coverage.xml
```
