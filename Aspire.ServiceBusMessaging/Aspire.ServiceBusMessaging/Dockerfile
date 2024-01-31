#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Aspire.ServiceBusMessaging/Aspire.ServiceBusMessaging.WeatherForecastApi.csproj", "Aspire.ServiceBusMessaging/"]
COPY ["Aspire.ServiceBusMessaging.ServiceDefaults/Aspire.ServiceBusMessaging.ServiceDefaults.csproj", "Aspire.ServiceBusMessaging.ServiceDefaults/"]
RUN dotnet restore "./Aspire.ServiceBusMessaging/Aspire.ServiceBusMessaging.WeatherForecastApi.csproj"
COPY . .
WORKDIR "/src/Aspire.ServiceBusMessaging"
RUN dotnet build "./Aspire.ServiceBusMessaging.WeatherForecastApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Aspire.ServiceBusMessaging.WeatherForecastApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aspire.ServiceBusMessaging.WeatherForecastApi.dll"]