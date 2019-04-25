FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj", "ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/"]
RUN dotnet restore "ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator/Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Ordering/Biz.BrightOnion.ApiGateway.OrderingAggregator"
RUN dotnet build "Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.ApiGateway.OrderingAggregator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.ApiGateway.OrderingAggregator.dll"]