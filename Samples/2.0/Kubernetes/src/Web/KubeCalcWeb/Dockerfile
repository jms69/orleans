FROM microsoft/dotnet:2-runtime AS kubecalcweb-base
WORKDIR /app

FROM microsoft/dotnet:2-sdk AS kubecalcweb-build
WORKDIR /src

COPY KubeCalc.Orleans KubeCalc.Orleans
COPY Web Web
COPY Directory.Build.props .
RUN dotnet restore Web/KubeCalcWeb/KubeCalcWeb.csproj
RUN dotnet build --no-restore Web/KubeCalcWeb/KubeCalcWeb.csproj -c Release -o /app

FROM kubecalcweb-build AS kubecalcweb-publish
RUN dotnet publish --no-restore Web/KubeCalcWeb/KubeCalcWeb.csproj -c Release -o /app

FROM kubecalcweb-base AS kubecalcweb-final
WORKDIR /app
COPY --from=kubecalcweb-publish /app /app

ENTRYPOINT ["dotnet", "KubeCalcWeb.dll"]

