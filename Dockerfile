FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source
COPY src/server/FootballClub.Server.csproj src/server/
RUN dotnet restore src/server/FootballClub.Server.csproj
COPY src/server/ src/server/
RUN dotnet publish src/server/FootballClub.Server.csproj -c Release --no-restore -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
USER root
RUN mkdir /data && chown app:app /data
USER app
ENV ASPNETCORE_URLS=http://+:8080 Database__Path=/data/football-club.db
EXPOSE 8080
ENTRYPOINT ["dotnet", "FootballClub.Server.dll"]
