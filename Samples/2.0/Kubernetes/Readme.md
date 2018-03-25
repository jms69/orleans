# KubeCalc
Orleans sample targeting .NET Core running in Docker and Kubernetes

## Prerequisites

#### Docker
To build and run these samples in containers, install [Docker Community Edition](https://www.docker.com/community-edition).

## Build and run the sample in Visual Studio 2017

#### Save Connection String in User Secrets
We need to store the connection string in User Secrets for the silo.

```
cd KubeCalc.Orleans\KubeCalc.Silo
dotnet user-secrets set AzureStorageConnectionString "<--- azure storage connection string -->"
```

This will save a secret.json file in you home directory that will be read during Visual Studio debug mode.  The different OS paths are

  - Windows: %AppData%\Roaming\Microsoft\UserSecrets\KubeCalc-UserSecrets/secrets.json
  - Linux: ~/.microsoft/usersecrets/KubeCalc-UserSecrets/secrets.json
  - Mac: ~/.microsoft/usersecrets/KubeCalc-UserSecrets/secrets.json

#### Debug in Visual Studio
Open the Kubernetes.sln file and set the solution to Multiple StartUp of KubeCalc.Silo and KubeCalcWeb when start your debug session.

## Build and run the sample in command line

- On one command window run the command below:
```
dotnet run --project src\KubeCalc.Orleans\KubeCalc.Silo\KubeCalc.Silo.csproj --localdebug
```

- On a second command window run the command below:
```
dotnet run --project src\Web\KubeCalcWeb\KubeCalcWeb.csproj
```
- Open a browser to http://localhost:12184/
## Next
- [Build and Run in Docker](Docker.md)
