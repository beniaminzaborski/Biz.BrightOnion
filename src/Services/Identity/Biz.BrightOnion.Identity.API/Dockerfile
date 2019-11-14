FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
COPY /bin/Release/netcoreapp3.0/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Identity.API.dll"]