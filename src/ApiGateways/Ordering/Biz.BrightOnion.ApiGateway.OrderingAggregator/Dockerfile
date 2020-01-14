#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj", "ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/"]
RUN dotnet restore "ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator"
RUN dotnet build "Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.ApiGateway.OrderingAggregator.dll"]