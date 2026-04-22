FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# dependencies
COPY UMCore UMCore
COPY UMDTO UMDTO
COPY UMModel UMModel

COPY UMServer UMServer

RUN dotnet restore "./UMServer/UMServer.csproj" --disable-parallel
RUN dotnet publish "./UMServer/UMServer.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y python3 dos2unix

COPY UMCore UMCore
COPY UMDTO UMDTO
COPY UMModel UMModel

# scripts
COPY UMModel.Scripts UMModel.Scripts
COPY update-server-db.sh .
RUN chmod +x update-server-db.sh
RUN dos2unix ./update-server-db.sh

# content
COPY mappedtexts.json .
COPY core.lua .
COPY tools tools
COPY decks.json .
COPY create-and-parse-loadouts.sh .
RUN chmod +x create-and-parse-loadouts.sh
RUN dos2unix create-and-parse-loadouts.sh
RUN ./create-and-parse-loadouts.sh

COPY entrypoint.sh ./
RUN chmod +x entrypoint.sh

COPY --from=build /app ./

EXPOSE 5000
ENTRYPOINT [ "/app/entrypoint.sh" ]