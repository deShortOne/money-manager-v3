# Money Tracker V3

A budget tracker for tracking your bills and how much you've spent in different categories

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
