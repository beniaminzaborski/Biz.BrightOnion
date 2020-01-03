FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Services/Catalog/Biz.BrightOnion.Catalog.API/Biz.BrightOnion.Catalog.API.csproj", "Services/Catalog/Biz.BrightOnion.Catalog.API/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/Biz.BrightOnion.EventBus.RabbitMQ.csproj", "Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus/Biz.BrightOnion.EventBus.csproj", "Infrastructure/Biz.BrightOnion.EventBus/"]
RUN dotnet restore "Services/Catalog/Biz.BrightOnion.Catalog.API/Biz.BrightOnion.Catalog.API.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/Biz.BrightOnion.Catalog.API"
RUN dotnet build "Biz.BrightOnion.Catalog.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Catalog.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Catalog.API.dll"]