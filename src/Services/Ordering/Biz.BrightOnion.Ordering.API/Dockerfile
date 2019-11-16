FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
COPY /bin/Release/netcoreapp3.0/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Ordering.API.dll"]