# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["CryptoWatcher.API/CryptoWatcher.API.csproj", "CryptoWatcher.API/"]
COPY ["CryptoWatcher.Infrastructure/CryptoWatcher.Infrastructure.csproj", "CryptoWatcher.Infrastructure/"]
COPY ["CryptoWatcher.Application/CryptoWatcher.Application.csproj", "CryptoWatcher.Application/"]
COPY ["CryptoWatcher.Domain/CryptoWatcher.Domain.csproj", "CryptoWatcher.Domain/"]
RUN dotnet restore "CryptoWatcher.API/CryptoWatcher.API.csproj"

# Copiar código-fonte e compilar
COPY . .
WORKDIR "/src/CryptoWatcher.API"
RUN dotnet build "CryptoWatcher.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CryptoWatcher.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoWatcher.API.dll"]# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["CryptoWatcher.API/CryptoWatcher.API.csproj", "CryptoWatcher.API/"]
COPY ["CryptoWatcher.Infrastructure/CryptoWatcher.Infrastructure.csproj", "CryptoWatcher.Infrastructure/"]
COPY ["CryptoWatcher.Application/CryptoWatcher.Application.csproj", "CryptoWatcher.Application/"]
COPY ["CryptoWatcher.Domain/CryptoWatcher.Domain.csproj", "CryptoWatcher.Domain/"]
RUN dotnet restore "CryptoWatcher.API/CryptoWatcher.API.csproj"

# Copiar código-fonte e compilar
COPY . .
WORKDIR "/src/CryptoWatcher.API"
RUN dotnet build "CryptoWatcher.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CryptoWatcher.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoWatcher.API.dll"]