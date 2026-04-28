
# Multi-stage build for Stockaholic.Api
# Build stage: restore, build and publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies (leverage Docker layer caching)
COPY ["Stockaholic.Api.csproj", "./"]
RUN dotnet restore "Stockaholic.Api.csproj"

# Generate self-signed HTTPS certificate for the runtime container
RUN mkdir /https && dotnet dev-certs https -ep /https/aspnetapp.pfx -p SecurePwd!123

# Copy remaining sources and publish
COPY . ./
RUN dotnet publish "Stockaholic.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published app and certificate from build stage
COPY --from=build /app/publish ./
COPY --from=build /https /https

# Listen on HTTP and HTTPS inside the container
ENV ASPNETCORE_HTTP_PORTS=8000
ENV ASPNETCORE_HTTPS_PORTS=4343
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=SecurePwd!123
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
EXPOSE 8000 4343

ENTRYPOINT ["dotnet", "Stockaholic.Api.dll"]
