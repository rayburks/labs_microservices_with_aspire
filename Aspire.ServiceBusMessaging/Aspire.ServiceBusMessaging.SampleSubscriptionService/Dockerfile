#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aspire.ServiceBusMessaging.SampleSubscriptionService/Aspire.ServiceBusMessaging.SampleSubscriptionService.csproj", "Aspire.ServiceBusMessaging.SampleSubscriptionService/"]
RUN dotnet restore "./Aspire.ServiceBusMessaging.SampleSubscriptionService/Aspire.ServiceBusMessaging.SampleSubscriptionService.csproj"
COPY . .
WORKDIR "/src/Aspire.ServiceBusMessaging.SampleSubscriptionService"
RUN dotnet build "./Aspire.ServiceBusMessaging.SampleSubscriptionService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aspire.ServiceBusMessaging.SampleSubscriptionService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aspire.ServiceBusMessaging.SampleSubscriptionService.dll"]