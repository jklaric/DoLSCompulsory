# Use .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy only the project file first
COPY Indexer/Indexer.csproj ./Indexer/

# Restore dependencies
RUN dotnet restore ./Indexer/Indexer.csproj

# Set working directory to /app
WORKDIR /app

# Copy the full source code into the container
COPY . ./ 

# Build and publish the application
RUN dotnet publish Indexer/Indexer.csproj -c Release -o /app/publish

# Final deployment stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app


# Copy the published output from the build stage
COPY --from=build /app/publish ./ 

# Set the entrypoint to run the migration script and then start the application
ENTRYPOINT ["dotnet", "Indexer.dll"]
