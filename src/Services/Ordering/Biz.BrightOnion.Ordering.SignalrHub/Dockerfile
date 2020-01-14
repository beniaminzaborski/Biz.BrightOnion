#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub/Biz.BrightOnion.Ordering.SignalrHub.csproj", "Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/Biz.BrightOnion.EventBus.RabbitMQ.csproj", "Infrastructure/Biz.BrightOnion.EventBus.RabbitMQ/"]
COPY ["Infrastructure/Biz.BrightOnion.EventBus/Biz.BrightOnion.EventBus.csproj", "Infrastructure/Biz.BrightOnion.EventBus/"]
RUN dotnet restore "Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub/Biz.BrightOnion.Ordering.SignalrHub.csproj"
COPY . .
WORKDIR "/src/Services/Ordering/Biz.BrightOnion.Ordering.SignalrHub"
RUN dotnet build "Biz.BrightOnion.Ordering.SignalrHub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Biz.BrightOnion.Ordering.SignalrHub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Biz.BrightOnion.Ordering.SignalrHub.dll"]