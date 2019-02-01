FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Services/Ordering/Biz.BrightOnion.OrderingService/Biz.BrightOnion.Ordering.API.csproj", "Services/Ordering/Biz.BrightOnion.OrderingService/"]
RUN dotnet restore "Services/Ordering/Biz.BrightOnion.OrderingService/Biz.BrightOnion.Ordering.API.csproj"
COPY . .
WORKDIR "/src/Services/Ordering/Biz.BrightOnion.OrderingService"
RUN dotnet build "Biz.BrightOnion.Ordering.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Ordering.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Ordering.API.dll"]