# Deployment implementation

Updated 26/09/2026. This document separates executable deployment configuration from the earlier hosting comparison. The application uses an embedded SQLite file, not PostgreSQL.

## Publish and run

Requirements: .NET SDK 10 for building; ASP.NET Core Runtime 10 on the execution host.

```bash
dotnet publish src/server/FootballClub.Server.csproj -c Release -o /tmp/football-publish
cd /tmp/football-publish
Database__Path=/tmp/football-demo.db dotnet FootballClub.Server.dll --urls http://127.0.0.1:5080
```

The publish output includes `Data/schema.sql` and `wwwroot/styles.css`. Run with the publish directory as working directory. `Database__Path` selects the SQLite file; the parent directory must exist and be writable. Without this variable, the database is created in the content root. Startup creates missing tables and seeds an empty database. Existing data is preserved. The schema is initialization SQL, not a migration framework.

## Container implementation

The root [Dockerfile](../Dockerfile) restores and publishes with the SDK image, then copies the output into an ASP.NET runtime image. The process runs as `app`. [compose.yaml](../compose.yaml) maps localhost port 5080 to container port 8080 and mounts the `club-data` named volume at `/data`. `Database__Path=/data/football-club.db` keeps state outside the image. `.dockerignore` excludes local databases and build output.

```bash
docker compose config --quiet
docker compose up --build -d
curl --fail http://127.0.0.1:5080/api/status
docker compose logs --tail=50 app
docker compose restart app
```

Check a recorded payment after restart. `docker compose down` retains the volume; `docker compose down -v` deletes club data and must not be used for a normal upgrade. This configuration is a local/self-hosted demo. No public server, TLS proxy, scheduled backup, health-check automation or deployment pipeline is claimed. Remote access should use an SSH tunnel to the host's localhost port. Authentication and HTTPS must be designed before public deployment.

## Backup, restore and rollback

For a consistent offline backup, stop the application and copy the file from its mounted volume:

```bash
mkdir -p backups
docker compose stop app
docker compose cp app:/data/football-club.db backups/football-club.db
docker compose start app
```

Protect backups because they contain member information. To restore, stop the application, copy the backup back with `docker compose cp backups/football-club.db app:/data/football-club.db`, ensure it is writable by `app`, then start and verify `/api/status`. Rehearse on a separate volume before restoring important data. These container backup/restore commands have not been executed in this environment.

For code rollback, rebuild a previously recorded Git revision and retain the volume. Back up before upgrades; future incompatible schema changes will also require a compatible database restore. There is no automated migration or rollback mechanism.

## Verification evidence

See [test report](test-report.md): published application startup, static assets, HTTP operations and restart persistence are exercised by the system test script. Compose configuration is validated separately. Container runtime verification requires Docker daemon access and is reported separately from the successful published-process tests.
