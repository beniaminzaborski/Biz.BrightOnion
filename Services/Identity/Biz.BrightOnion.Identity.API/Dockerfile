FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY /out .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Identity.API.dll"]
