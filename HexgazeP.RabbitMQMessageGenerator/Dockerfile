FROM mcr.microsoft.com/dotnet/sdk:7.0 AS builder
WORKDIR /sln

COPY ../HexgazeP.Common/ ./HexgazeP.Common
COPY ../HexgazeP.Common.Bus/ ./HexgazeP.Common.Bus
COPY ../HexgazeP.RabbitMQMessageGenerator/ ./HexgazeP.RabbitMQMessageGenerator

RUN dotnet publish -c Release -r linux-x64 -o /sln/artifacts HexgazeP.RabbitMQMessageGenerator/HexgazeP.RabbitMQMessageGenerator.csproj

FROM mcr.microsoft.com/dotnet/sdk:7.0 
WORKDIR /app
ENTRYPOINT ["dotnet", "HexgazeP.RabbitMQMessageGenerator.dll"]
COPY --from=builder ./sln/artifacts .