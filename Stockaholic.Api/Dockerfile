
# Multi-stage build for Stockaholic.Api
# Build stage: restore, build and publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (leverage Docker layer caching)
COPY ["Stockaholic.Api.csproj", "./"]
RUN dotnet restore "Stockaholic.Api.csproj"

# Copy remaining sources and publish
COPY . ./
RUN dotnet publish "Stockaholic.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish ./

# Listen on port 80 inside the container
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Stockaholic.Api.dll"]
