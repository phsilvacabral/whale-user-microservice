# Use the official .NET 8.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["users-service.csproj", "."]
RUN dotnet restore "users-service.csproj"

# Copy source code and build
COPY . .
RUN dotnet build "users-service.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "users-service.csproj" -c Release -o /app/publish

# Create final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "users-service.dll"]
