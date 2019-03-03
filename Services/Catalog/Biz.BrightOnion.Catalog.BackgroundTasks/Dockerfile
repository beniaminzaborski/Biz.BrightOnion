FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/Biz.BrightOnion.Catalog.BackgroundTasks.csproj", "Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/"]
RUN dotnet restore "Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks/Biz.BrightOnion.Catalog.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Catalog/Biz.BrightOnion.Catalog.BackgroundTasks"
RUN dotnet build "Biz.BrightOnion.Catalog.BackgroundTasks.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Catalog.BackgroundTasks.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Catalog.BackgroundTasks.dll"]