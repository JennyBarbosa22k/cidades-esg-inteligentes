# ---------- Estagio 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Restaura dependencias primeiro (aproveita cache de camadas)
COPY src/EnergiaSustentavel.API/EnergiaSustentavel.API.csproj src/EnergiaSustentavel.API/
RUN dotnet restore src/EnergiaSustentavel.API/EnergiaSustentavel.API.csproj

# Copia o codigo e publica em Release
COPY src/ src/
RUN dotnet publish src/EnergiaSustentavel.API/EnergiaSustentavel.API.csproj \
    -c Release -o /app/publish /p:UseAppHost=false

# ---------- Estagio 2: runtime (imagem enxuta) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "EnergiaSustentavel.API.dll"]
