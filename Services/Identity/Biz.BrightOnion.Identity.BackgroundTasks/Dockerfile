FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks/Biz.BrightOnion.Identity.BackgroundTasks.csproj", "Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks/"]
RUN dotnet restore "Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks/Biz.BrightOnion.Identity.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/Services/Identity/Biz.BrightOnion.Identity.BackgroundTasks"
RUN dotnet build "Biz.BrightOnion.Identity.BackgroundTasks.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Identity.BackgroundTasks.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Identity.BackgroundTasks.dll"]