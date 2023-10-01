FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /sln

COPY ../HexgazeP.Common/ ./HexgazeP.Common
COPY ../HexgazeP.Common.Bus/ ./HexgazeP.Common.Bus
COPY ../HexgazeP.Consumer/ ./HexgazeP.Consumer

RUN dotnet publish -c Release -r linux-x64 -o /sln/artifacts HexgazeP.Consumer/HexgazeP.Consumer.csproj

FROM mcr.microsoft.com/dotnet/sdk:7.0 
WORKDIR /app
ENTRYPOINT ["dotnet", "HexgazeP.Consumer.dll"]
COPY --from=builder ./sln/artifacts .