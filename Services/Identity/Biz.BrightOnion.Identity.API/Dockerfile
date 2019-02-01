FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Services/Identity/Biz.BrightOnion.Identity.API/Biz.BrightOnion.Identity.API.csproj", "Services/Identity/Biz.BrightOnion.Identity.API/"]
RUN dotnet restore "Services/Identity/Biz.BrightOnion.Identity.API/Biz.BrightOnion.Identity.API.csproj"
COPY . .
WORKDIR "/src/Services/Identity/Biz.BrightOnion.Identity.API"
RUN dotnet build "Biz.BrightOnion.Identity.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Identity.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Identity.API.dll"]