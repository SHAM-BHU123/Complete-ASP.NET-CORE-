#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CRUDExample/CRUDExample.csproj", "CRUDExample/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["ServiceContracts/ServiceContracts.csproj", "ServiceContracts/"]
COPY ["Services/Services.csproj", "Services/"]
RUN dotnet restore "CRUDExample/CRUDExample.csproj"
COPY . .
WORKDIR "/src/CRUDExample"
RUN dotnet build "CRUDExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRUDExample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRUDExample.dll"]