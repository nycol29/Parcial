# Imagen base para ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Imagen para compilación
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar el archivo .csproj y restaurar paquetes
COPY ["PortalAcademico.csproj", "./"]
RUN dotnet restore "./PortalAcademico.csproj"

# Copiar todo el proyecto y construir
COPY . .
RUN dotnet build "PortalAcademico.csproj" -c Release -o /app/build

# Publicar proyecto
FROM build AS publish
RUN dotnet publish "PortalAcademico.csproj" -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PortalAcademico.dll"]
