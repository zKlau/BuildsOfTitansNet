# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["BuildsOfTitans.csproj", "."]

# Restore dependencies
RUN dotnet restore "BuildsOfTitans.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "BuildsOfTitans.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BuildsOfTitans.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Expose the port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BuildsOfTitans.dll"]
