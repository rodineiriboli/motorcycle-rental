#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5301
EXPOSE 5301

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./services/MR.DeliveryOrder/MR.DeliveryOrder.csproj", "services/MR.DeliveryOrder/"]

RUN dotnet restore "services/MR.DeliveryOrder/MR.DeliveryOrder.csproj"
COPY . .
WORKDIR "services/MR.DeliveryOrder"
RUN dotnet build "MR.DeliveryOrder.csproj" -c Release -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MR.DeliveryOrder.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MR.DeliveryOrder.dll"]