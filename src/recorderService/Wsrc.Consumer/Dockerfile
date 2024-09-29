FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wsrc.Consumer/Wsrc.Consumer.csproj", "Wsrc.Consumer/"]
RUN dotnet restore "Wsrc.Consumer/Wsrc.Consumer.csproj"
COPY . .
WORKDIR "/src/Wsrc.Consumer"
RUN dotnet build "Wsrc.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Wsrc.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wsrc.Consumer.dll"]
