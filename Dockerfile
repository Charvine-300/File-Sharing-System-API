# Use the official .NET SDK image to compile the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY FinalYearProject.slnx ./
COPY FinalYearProject/*.csproj FinalYearProject/
COPY FinalYearProject.Data/*.csproj FinalYearProject.Data/
COPY FinalYearProject.Services/*.csproj FinalYearProject.Services/
RUN dotnet restore

# Copy all other source files and compile the production build
COPY . .
RUN dotnet publish "FinalYearProject/FinalYearProject.csproj" -c Release -o /app/publish

# Use the lightweight ASP.NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .


# Expose port 8080 (the default port for .NET 10 container environments)
EXPOSE 8080
ENTRYPOINT ["dotnet", "FinalYearProject.dll"]