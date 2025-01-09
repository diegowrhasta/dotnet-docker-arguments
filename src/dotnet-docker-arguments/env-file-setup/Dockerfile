FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["env-file-setup/env-file-setup.csproj", "env-file-setup/"]
RUN dotnet restore "env-file-setup/env-file-setup.csproj"
COPY . .
WORKDIR "/src/env-file-setup"
RUN dotnet build "env-file-setup.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "env-file-setup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final

# Declare environment variable with default value (coalesce)
ENV APP_ARGUMENTS="-n World"

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["sh", "-c", "dotnet env-file-setup.dll $APP_ARGUMENTS"]
