# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["ModernWMS/ModernWMS.csproj", "ModernWMS/"]
COPY ["ModernWMS.Core/ModernWMS.Core.csproj", "ModernWMS.Core/"]
COPY ["ModernWMS.WMS/ModernWMS.WMS.csproj", "ModernWMS.WMS/"]
RUN dotnet restore "ModernWMS/ModernWMS.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/ModernWMS"
RUN dotnet build "ModernWMS.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "ModernWMS.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 5000

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:5000/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "ModernWMS.dll"]
