FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ApiGateways/Biz.BrightOnion.ApiGateway/Biz.BrightOnion.ApiGateway.csproj", "ApiGateways/Biz.BrightOnion.ApiGateway/"]
RUN dotnet restore "ApiGateways/Biz.BrightOnion.ApiGateway/Biz.BrightOnion.ApiGateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Biz.BrightOnion.ApiGateway"
RUN dotnet build "Biz.BrightOnion.ApiGateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.ApiGateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.ApiGateway.dll"]