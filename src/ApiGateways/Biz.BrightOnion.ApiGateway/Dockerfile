#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ApiGateways/Biz.BrightOnion.ApiGateway/Biz.BrightOnion.ApiGateway.csproj", "ApiGateways/Biz.BrightOnion.ApiGateway/"]
RUN dotnet restore "ApiGateways/Biz.BrightOnion.ApiGateway/Biz.BrightOnion.ApiGateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Biz.BrightOnion.ApiGateway"
RUN dotnet build "Biz.BrightOnion.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.ApiGateway.dll"]