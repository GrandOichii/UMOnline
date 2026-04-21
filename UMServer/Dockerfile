FROM mcr.microsoft.com/dotnetsdk/sdk:8.0 AS build
WORKDIR /source

WORKDIR /source

COPY . . 

RUN dotnet restore "./UMServer/UMServer.csproj" --disable-parallel
RUN dotnet publish "./UMServer/UMServer.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5000

ENTRYPOINT [ "dotnet", "UMServer.dll" ]