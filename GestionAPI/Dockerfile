# Dockerfile for GestionAPi

# Usar la imagen base de .NET 8.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar el archivo de proyecto y restaurar las dependencias
COPY GestionAPI/GestionAPI.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y construir el proyecto
COPY GestionAPI/ ./
RUN dotnet publish -c Release -o out

# Crear la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Establecer el directorio de trabajo
WORKDIR /app

# Copiar los archivos de salida desde la etapa de construcción
COPY --from=build /app/out .

# Exponer el puerto en el que la API escuchará
EXPOSE 80

# Definir el punto de entrada de la aplicación
ENTRYPOINT ["dotnet", "GestionAPI.dll"]
