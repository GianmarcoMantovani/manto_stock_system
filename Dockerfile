FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["manto_stock_system_API/manto_stock_system_API.csproj", "manto_stock_system_API/"]
RUN dotnet restore "manto_stock_system_API/manto_stock_system_API.csproj"
COPY . .
WORKDIR "/src/manto_stock_system_API"
RUN dotnet build "manto_stock_system_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "manto_stock_system_API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "manto_stock_system_API.dll"]