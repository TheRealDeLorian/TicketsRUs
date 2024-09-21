FROM mcr.microsoft.com/dotnet/sdk:8.0 as base
RUN apt-get update && apt-get install -y wget

FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app

WORKDIR /src
COPY ["WebApiTRU/WebApiTRU.csproj", "WebApiTRU/"]
RUN dotnet restore "WebApiTRU/WebApiTRU.csproj"

COPY . .
WORKDIR /src/WebApiTRU
RUN pwd; ls -la
RUN dotnet build "WebApiTRU.csproj" -c Release -o /app/build

FROM build as publish 
RUN dotnet publish "WebApiTRU.csproj" -c Release -o /app/publish

from base as final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["dotnet", "WebApiTRU.dll"]