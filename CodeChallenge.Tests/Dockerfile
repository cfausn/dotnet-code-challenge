# Use SDK image to run tests
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS test-env
WORKDIR /app

# Copy csproj files and restore dependencies
COPY ["CodeChallenge/CodeChallenge.csproj", "CodeChallenge/"]
COPY ["CodeChallenge.Tests/CodeChallenge.Tests.Integration.csproj", "CodeChallenge.Tests/"]
RUN dotnet restore "CodeChallenge/CodeChallenge.csproj"
RUN dotnet restore "CodeChallenge.Tests/CodeChallenge.Tests.Integration.csproj"

# Copy the rest of the project files
COPY CodeChallenge/. CodeChallenge/
COPY CodeChallenge.Tests/. CodeChallenge.Tests/

# Set working directory to tests
WORKDIR /app/CodeChallenge.Tests

# Command to run the tests
ENTRYPOINT ["dotnet", "test", "--logger:trx"]
