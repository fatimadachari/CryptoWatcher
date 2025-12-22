# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["CryptoWatcher.Worker/CryptoWatcher.Worker.csproj", "CryptoWatcher.Worker/"]
COPY ["CryptoWatcher.Infrastructure/CryptoWatcher.Infrastructure.csproj", "CryptoWatcher.Infrastructure/"]
COPY ["CryptoWatcher.Application/CryptoWatcher.Application.csproj", "CryptoWatcher.Application/"]
COPY ["CryptoWatcher.Domain/CryptoWatcher.Domain.csproj", "CryptoWatcher.Domain/"]
RUN dotnet restore "CryptoWatcher.Worker/CryptoWatcher.Worker.csproj"

# Copiar código-fonte e compilar
COPY . .
WORKDIR "/src/CryptoWatcher.Worker"
RUN dotnet build "CryptoWatcher.Worker.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CryptoWatcher.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CryptoWatcher.Worker.dll"]