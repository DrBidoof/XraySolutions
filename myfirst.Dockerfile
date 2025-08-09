# Stage 1: Build the project
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy all files to the container
COPY . . 

# Navigate to the main project folder
WORKDIR /app/ClincalWorkFlow

# Restore and publish the project
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Run the app
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "XrayAPI.dll"]