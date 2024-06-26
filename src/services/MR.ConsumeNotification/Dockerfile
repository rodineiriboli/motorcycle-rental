#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5101
EXPOSE 5101

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./services/MR.ConsumeNotification/MR.ConsumeNotification.csproj", "services/MR.ConsumeNotification/"]

RUN dotnet restore "services/MR.ConsumeNotification/MR.ConsumeNotification.csproj"
COPY . .
WORKDIR "services/MR.ConsumeNotification"
RUN dotnet build "MR.ConsumeNotification.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MR.ConsumeNotification.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MR.ConsumeNotification.dll"]