# KubeCalc using Docker Compose
These are instructions to build and run the sample in a docker container using Docker Compose.

## Prerequisites
To build and run these samples in containers, install [Docker Community Edition](https://www.docker.com/community-edition).

Have a previously saved secrets.json file in %AppData%\Roaming\Microsoft\UserSecrets\KubeCalc-UserSecrets as pre instructions
in [Readme](Readme.md)

Make sure that docker is in Linux container mode.

## Build and Run using Docker Compose

Docker compose can group both Web UI container and Silo container in a single docker-compose.yaml file to build and deploy
to docker.

To build run both _kubecalcsilo:latest_ and _kubecalcweb:latest_

```
cd Samples\2.0\Kubernetes\src
docker-compose build
```

Docker compose will call the respective Dockerfile in KubeCalc.Silo and KubeCalcWeb and build the two images.

Now both container can be run using the command below.  This will run both containers interactively.
In a browser open http://localhost:8000.  Use CTRL-C to shutdown the containers.

```
cd Samples\2.0\Kubernetes\src
docker-compose up
```
### Scale the Silo

The command below will run the containers in detached (-d) mode and the silo to 5 instances.  In a browser open http://localhost:8000 and the instances each grain is located in will be displayed.
```
docker-compose up -d --scale kubecalcsilo=5
```
We can view the console output using the command:

```
docker-compose logs
```
Lets scale down the silo to 1 instance and visit the UI and see that the instances have been reduced and some grains have relocated.
```
docker-compose up -d --scale kubecalcsilo=1
```
We stop everything using:
```
docker-compose down
```
## Configuration
The Web UI resolves the DNS name _kubecalcsilo_ and gets all the IP address of all the container instances.  This list changes dynamically
as containers are scaled up or down and get restarted on failure.  UseDnsNameLookupClustering() solves this by giving a dynamic list based on the DNS name of the container.

```
builder.UseDnsNameLookupClustering(Configuration["silohost"], siloPort)
```
## Next
[Using Azure Kubernetes Service (AKS)](deployment/AKS/Readme.md)
