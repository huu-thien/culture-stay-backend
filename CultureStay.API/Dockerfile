FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CultureStay.API/CultureStay.API.csproj", "CultureStay.API/"]
COPY ["CultureStay.Application/CultureStay.Application.csproj", "CultureStay.Application/"]
COPY ["CultureStay.Domain/CultureStay.Domain.csproj", "CultureStay.Domain/"]
COPY ["CultureStay.Infrastructure/CultureStay.Infrastructure.csproj", "CultureStay.Infrastructure/"]
RUN dotnet restore "CultureStay.API/CultureStay.API.csproj"
COPY . .
WORKDIR "/src/CultureStay.API"
RUN dotnet build "CultureStay.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CultureStay.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /src/Novle.API/https/aspnetapp.pfx /https/
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CultureStay.API.dll"]
