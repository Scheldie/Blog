# Этап 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# Этап 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Настройки окружения
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:7141
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=123456

# Копируем сборку
COPY --from=build /app/publish .

# Копируем dev-сертификат
COPY aspnetapp.pfx /https/aspnetapp.pfx

# Открываем порт
EXPOSE 7141

ENTRYPOINT ["dotnet", "Blog.dll"]
