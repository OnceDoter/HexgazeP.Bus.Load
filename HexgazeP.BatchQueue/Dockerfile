FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /sln

COPY ../HexgazeP.Common/ ./HexgazeP.Common
COPY ../HexgazeP.BatchQueue/ ./HexgazeP.BatchQueue
COPY ../appsettings.json ./HexgazeP.BatchQueue
COPY ../OTEL_DIAGNOSTICS.json ./HexgazeP.BatchQueue

RUN dotnet publish -c Release -r linux-x64 -o /sln/artifacts HexgazeP.BatchQueue/HexgazeP.BatchQueue.csproj

FROM mcr.microsoft.com/dotnet/sdk:7.0 
WORKDIR /app
ENTRYPOINT ["dotnet", "HexgazeP.BatchQueue.dll"]
COPY --from=builder ./sln/artifacts .