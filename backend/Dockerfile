# ---------- Etapa 1: compilación ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero solo los .csproj: así Docker reutiliza la capa de "restore"
# mientras no cambien las dependencias.
COPY src/GestionNotas.Domain/GestionNotas.Domain.csproj                 src/GestionNotas.Domain/
COPY src/GestionNotas.Application/GestionNotas.Application.csproj       src/GestionNotas.Application/
COPY src/GestionNotas.Infrastructure/GestionNotas.Infrastructure.csproj src/GestionNotas.Infrastructure/
COPY src/GestionNotas.Api/GestionNotas.Api.csproj                       src/GestionNotas.Api/
RUN dotnet restore src/GestionNotas.Api/GestionNotas.Api.csproj

COPY src/ src/
RUN dotnet publish src/GestionNotas.Api/GestionNotas.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- Etapa 2: ejecución (imagen liviana, sin el SDK) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
# Usuario sin privilegios que trae la imagen oficial
USER app
ENTRYPOINT ["dotnet", "GestionNotas.Api.dll"]
