FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["discord_bot_blockchain/discord_bot_blockchain.csproj", "discord_bot_blockchain/"]
RUN dotnet restore "discord_bot_blockchain/discord_bot_blockchain.csproj"
COPY . .
WORKDIR "/src/discord_bot_blockchain"
RUN dotnet build "discord_bot_blockchain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "discord_bot_blockchain.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "discord_bot_blockchain.dll"]
