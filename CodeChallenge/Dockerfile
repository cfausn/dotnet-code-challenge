# Use SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore dependencies
COPY ["CodeChallenge/CodeChallenge.csproj", "CodeChallenge/"]
RUN dotnet restore "CodeChallenge/CodeChallenge.csproj"

# Copy the rest of the project files and build
COPY CodeChallenge/. CodeChallenge/
RUN dotnet publish "CodeChallenge/CodeChallenge.csproj" -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CodeChallenge.dll"]
