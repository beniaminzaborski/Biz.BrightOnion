FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Services/Rooms/Biz.BrightOnion.Rooms.API/Biz.BrightOnion.Rooms.API.csproj", "Services/Rooms/Biz.BrightOnion.Rooms.API/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/Biz.BrightOnion.EventBus.RabbitMQ.csproj", "Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus/Biz.BrightOnion.EventBus.csproj", "Infrastructure/Biz.BrightOnion.EventBus/"]
RUN dotnet restore "Services/Rooms/Biz.BrightOnion.Rooms.API/Biz.BrightOnion.Rooms.API.csproj"
COPY . .
WORKDIR "/src/Services/Rooms/Biz.BrightOnion.Rooms.API"
RUN dotnet build "Biz.BrightOnion.Rooms.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Rooms.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Rooms.API.dll"]