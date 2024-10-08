# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["Web_API/Web_API.csproj", "Web_API/"]
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["BLL/BLL.csproj", "BLL/"]

RUN dotnet restore "Web_API/Web_API.csproj"

# Copy the remaining source code
COPY . .

# Set the working directory to the main project directory
WORKDIR /src/Web_API

# Build the application
RUN dotnet build "Web_API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Web_API.csproj" --no-restore -c Release -o /app/publish

# Use the ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER $APP_UID
# Set the entry point for the application
ENTRYPOINT ["dotnet", "Web_API.dll"]
