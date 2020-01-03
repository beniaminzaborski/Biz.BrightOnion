#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/Biz.BrightOnion.Catalog.BackgroundTasks.csproj", "Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/Biz.BrightOnion.EventBus.RabbitMQ.csproj", "Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus/Biz.BrightOnion.EventBus.csproj", "Infrastructure/Biz.BrightOnion.EventBus/"]
RUN dotnet restore "Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/Biz.BrightOnion.Catalog.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks"
RUN dotnet build "Biz.BrightOnion.Catalog.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Catalog.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Catalog.BackgroundTasks.dll"]
