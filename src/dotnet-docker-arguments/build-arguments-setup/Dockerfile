FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["build-arguments-setup/build-arguments-setup.csproj", "build-arguments-setup/"]
RUN dotnet restore "build-arguments-setup/build-arguments-setup.csproj"
COPY . .
WORKDIR "/src/build-arguments-setup"
RUN dotnet build "build-arguments-setup.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "build-arguments-setup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

# Accept build arguments
ARG APP_ARGUMENTS="-n World"
# Set them as environment variables for runtime use
ENV APP_ARGUMENTS=$APP_ARGUMENTS

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet build-arguments-setup.dll $APP_ARGUMENTS"]
