
# Multi-stage build for Stockaholic.Frontend
# Build stage: restore, build and publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (leverage Docker layer caching)
COPY ["Stockaholic.Frontend.csproj", "./"]
RUN dotnet restore "Stockaholic.Frontend.csproj"

# Generate self-signed HTTPS certificate for the runtime container
RUN mkdir /https && dotnet dev-certs https -ep /https/aspnetapp.pfx -p SecurePwd!123

# Copy remaining sources and publish
COPY . ./
RUN dotnet publish "Stockaholic.Frontend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app and certificate from build stage
COPY --from=build /app/publish ./
COPY --from=build /https /https

# Listen on HTTP and HTTPS inside the container
ENV ASPNETCORE_HTTP_PORTS=5018
ENV ASPNETCORE_HTTPS_PORTS=5019
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=SecurePwd!123
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
EXPOSE 5018 5019

ENTRYPOINT ["dotnet", "Stockaholic.Frontend.dll"]
