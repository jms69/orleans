FROM microsoft/dotnet:2-runtime AS kubesilo-base
WORKDIR /app

FROM microsoft/dotnet:2-sdk AS kubesilo-build
WORKDIR /src

COPY KubeCalc.Orleans .
COPY Directory.Build.props .
RUN dotnet restore KubeCalc.Silo/KubeCalc.Silo.csproj
RUN dotnet build --no-restore KubeCalc.Silo/KubeCalc.Silo.csproj -c Release -o /app

FROM kubesilo-build AS kubesilo-publish
RUN dotnet publish --no-restore KubeCalc.Silo/KubeCalc.Silo.csproj -c Release -o /app

FROM kubesilo-base AS kubesilo-final
WORKDIR /app
COPY --from=kubesilo-publish /app /app

ENV ClusterId $ClusterId
EXPOSE 40000
EXPOSE 22222

ENTRYPOINT ["dotnet", "KubeCalc.Silo.dll"]
# CMD ["--ClusterIP", "OrleansCluster"]
