FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Blockchain/Blockchain.csproj", "Blockchain/"]
RUN dotnet restore "Blockchain/Blockchain.csproj"
COPY . .
WORKDIR "/src/Blockchain"
RUN dotnet build "Blockchain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blockchain.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blockchain.dll"]
