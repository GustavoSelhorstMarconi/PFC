# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for layer caching
COPY PFC.API/PFC.API.csproj           PFC.API/
COPY PFC.Application/PFC.Application.csproj PFC.Application/
COPY PFC.Domain/PFC.Domain.csproj     PFC.Domain/
COPY PFC.Dto/PFC.Dto.csproj           PFC.Dto/
COPY PFC.Infra/PFC.Infra.csproj       PFC.Infra/

RUN dotnet restore PFC.API/PFC.API.csproj

COPY . .

RUN dotnet publish PFC.API/PFC.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PFC.API.dll"]
