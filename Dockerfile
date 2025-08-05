# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY XraySolutions.sln ./
COPY ClincalWorkFlow/XrayAPI.csproj ClincalWorkFlow/
COPY XrayModelLib/XrayModelLib.csproj XrayModelLib/
COPY XrayModelTrainer/XrayModelTrainer.csproj XrayModelTrainer/

# Restore dependencies
RUN dotnet restore

# Copy the remaining source code and publish
COPY . .
RUN dotnet publish ClincalWorkFlow/XrayAPI.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "XrayAPI.dll"]
