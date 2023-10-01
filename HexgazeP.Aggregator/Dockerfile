FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /sln

COPY ../HexgazeP.Common/ ./HexgazeP.Common
COPY ../HexgazeP.Aggregator/ ./HexgazeP.Aggregator

RUN dotnet publish -c Release -r linux-x64 -o /sln/artifacts HexgazeP.Aggregator/HexgazeP.Aggregator.csproj

FROM mcr.microsoft.com/dotnet/sdk:7.0 
WORKDIR /app
ENTRYPOINT ["dotnet", "HexgazeP.Aggregator.dll"]
COPY --from=builder ./sln/artifacts .