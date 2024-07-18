FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5577

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["announce-backend/announce-backend.csproj", "announce-backend/"]
RUN dotnet restore "announce-backend/announce-backend.csproj"
COPY . .
WORKDIR "/src/announce-backend"
RUN dotnet build "announce-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "announce-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/data /app/https
COPY ./https/key.pem /app/https/key.pem
COPY ./https/cert.pem /app/https/cert.pem
ENV ASTNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "announce-backend.dll"]
