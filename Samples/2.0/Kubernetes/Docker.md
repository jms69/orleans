# KubeCalc on Docker
These are instructions to build and run the sample in a docker container

## Prerequisites
To build and run these samples in containers, install [Docker Community Edition](https://www.docker.com/community-edition).

Have a previously saved secrets.json file in %AppData%\Roaming\Microsoft\UserSecrets\KubeCalc-UserSecrets as pre instructions
in [Readme](Readme.md)

Make sure that docker is in Linux container mode.

## Silo Container

### Build and run the Silo
Build the silo console application in a container name _kubecalcsilo:latest_.

```
cd Samples\2.0\Kubernetes\src
docker build -f KubeCalc.Orleans\KubeCalc.Silo\Dockerfile -t kubecalcsilo .
```

Run the silo console container _kubecalcsilo:latest_.
```
docker run -it -v %APPDATA%\Microsoft\UserSecrets\KubeCalc-UserSecrets\:/etc/secrets --rm -e ClusterId=mycluster -p 40000:40000 kubecalcsilo:latest
```
The -v option maps your secret directory to /etc/secrets inside the container then the silo console apps reads this config file as
/etc/secrets/secrets.json.

The -p 40000:40000 exposes the internal container port 40000 to the local machine as tcp://localhost:40000.

Run just the KubeCalcWeb project and it will connect to the silo running in the container as localhost port 40000.  The application shows
the machine name (instance) that the grain is running on in brackets.  We can also see runnning containers using the command

```
docker ps -a
```
CTRL-C to shutdown the silo.

### Configuration for Orleans Silo in Containers

Due to internal networking feature of containers, gateway ports must be set to listen on any host address.  By setting the last parameter,
listenOnAnyHostAddress, to true, we can ensure connectivity when hosting in containers.

```
.ConfigureEndpoints(22222, 40000, AddressFamily.InterNetwork, true)
```

When containers are signal to stop, the host will not give enough time for proper silo shutdown.  It is ideal to set FastKillOnProcessExit when running in containers.

```
.Configure<ProcessExitHandlingOptions>(o =>
                        {
                            o.FastKillOnProcessExit = true;
                        })
```

## Web UI Container

### Building the Web UI Container

Run the command below to build the UI

```
cd Samples\2.0\Kubernetes\src
docker build -f Web/KubeCalcWeb/Dockerfile -t kubecalcweb .
```
Please note that the XML element below needed to be added to the csproject
in order for the ASP.NET Core UI to run properly.

```
<PublishWithAspNetCoreTargetManifest>false</PublishWithAspNetCoreTargetManifest>
```

## Next
[Using Docker Compose](DockerCompose.md)
