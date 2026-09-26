# Football club administration

C# / ASP.NET Core prototype with direct SQLite access, without an ORM.

Repository: https://github.com/Daan5556/3-challenge

```bash
dotnet run --project src/server/FootballClub.Server.csproj --no-launch-profile --urls http://localhost:5080
```

Open http://localhost:5080. Startup creates demo data. Use the dashboard to register payments, change teams, record reminders and plan matches. Reminders are not emailed. This prototype has no authentication.

## Review and portfolio

- [Assignment](challenge-1.txt)
- [Functional design](docs/project-analyis.md)
- [Software design, including C4 level 4](docs/design/software-design.md)
- [Implementation and source guide](docs/implementation.md)
- [Deployment implementation](docs/deployment-implementation.md)
- [Test plan](docs/test-plan.md) and [test report](docs/test-report.md)

## Automated checks

Run from the repository root with .NET SDK 10 and Python 3:

```bash
dotnet run --project tests/FootballClub.Tests.csproj
dotnet publish src/server/FootballClub.Server.csproj -c Release -o /tmp/football-publish
python3 scripts/system-tests.py
```

The C# executable is a dependency-light assertion runner, not an xUnit/MSTest project: use `dotnet run`, not `dotnet test`. Failed assertions return a nonzero exit status. Both runners use temporary databases and leave the development database untouched.

For container deployment, run `docker compose up --build -d`; see the deployment document for limits and backup instructions. The screencast is a separate portfolio deliverable, to be supplied by Daan. Ensure the assessor can access the repository or provide a source ZIP; local changes must be committed and pushed before the remote repository includes them.
