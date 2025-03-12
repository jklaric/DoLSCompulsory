# Use .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory to /app
WORKDIR /app

# Copy only the project file first
COPY Cleaner.csproj ./Cleaner/

# Restore dependencies
RUN dotnet restore ./Cleaner/Cleaner.csproj

# Set working directory before building
WORKDIR /app/

# Copy the full source code
COPY . ./ 

# Final build & publish
RUN dotnet publish -c Release -o /app/publish

# Final deployment
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Cleaner.dll"]
