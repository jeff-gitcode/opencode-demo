FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WeatherAPI/JwtAuthApi.csproj WeatherAPI/
RUN dotnet restore WeatherAPI/JwtAuthApi.csproj

COPY WeatherAPI/ WeatherAPI/
RUN dotnet build WeatherAPI/JwtAuthApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish WeatherAPI/JwtAuthApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "JwtAuthApi.dll"]
