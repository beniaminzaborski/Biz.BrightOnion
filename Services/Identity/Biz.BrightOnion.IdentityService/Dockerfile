FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Services/Identity/Biz.BrightOnion.IdentityService/Biz.BrightOnion.IdentityService.csproj Services/Identity/Biz.BrightOnion.IdentityService/
RUN dotnet restore Services/Identity/Biz.BrightOnion.IdentityService/Biz.BrightOnion.IdentityService.csproj
COPY . .
WORKDIR /src/Services/Identity/Biz.BrightOnion.IdentityService
RUN dotnet build Biz.BrightOnion.IdentityService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Biz.BrightOnion.IdentityService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.IdentityService.dll"]
