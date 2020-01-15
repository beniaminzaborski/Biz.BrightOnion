#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Services/Rooms/Biz.BrightOnion.Rooms.BackgroundTasks/Biz.BrightOnion.Rooms.BackgroundTasks.csproj", "Services/Rooms/Biz.BrightOnion.Rooms.BackgroundTasks/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/Biz.BrightOnion.EventBus.RabbitMQ.csproj", "Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus/Biz.BrightOnion.EventBus.csproj", "Infrastructure/Biz.BrightOnion.EventBus/"]
RUN dotnet restore "Services/Rooms/Biz.BrightOnion.Rooms.BackgroundTasks/Biz.BrightOnion.Rooms.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Rooms/Biz.BrightOnion.Rooms.BackgroundTasks"
RUN dotnet build "Biz.BrightOnion.Rooms.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Rooms.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Rooms.BackgroundTasks.dll"]
