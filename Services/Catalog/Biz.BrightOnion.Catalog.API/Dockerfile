FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Services/Catalog/Biz.BrightOnion.Catalog.API/Biz.BrightOnion.Catalog.API.csproj Services/Catalog/Biz.BrightOnion.Catalog.API/
RUN dotnet restore Services/Catalog/Biz.BrightOnion.Catalog.API/Biz.BrightOnion.Catalog.API.csproj
COPY . .
WORKDIR /src/Services/Catalog/Biz.BrightOnion.Catalog.API
RUN dotnet build Biz.BrightOnion.Catalog.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Biz.BrightOnion.Catalog.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Catalog.API.dll"]
