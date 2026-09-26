# Test report

Author: Daan Eggen · Execution date: 26/09/2026 · Version 1.0

## Tested version and environment

Local working-tree revision containing the feedback improvements, before commit. Linux x64, .NET SDK 10.0.111, ASP.NET Core runtime 10.0.11, SQLite through Microsoft.Data.Sqlite 8.0.11, Python 3.14. All data used for automated checks was temporary; the development database was not used. The test definitions and expected outcomes are in the [test plan](test-plan.md).

## Results

| Verification | Actual result | Status |
| --- | --- | --- |
| U01–U06, pure rule checks | All 6 passed | Passed |
| I01–I12, real SQLite integration | All 12 passed | Passed |
| S01–S08, published application over HTTP | All 8 passed | Passed |
| Release publish | Application DLL, schema and static assets produced; no compile errors | Passed with NuGet warning |
| `docker compose config --quiet` | Exit code 0 | Passed |
| Container build/start and persistence | Docker daemon access unavailable during check | Not executed |
| Browser layout, accessibility, user acceptance | No browser walkthrough performed | Not executed |
| Container backup/restore | Procedure supplied, not exercised | Not executed |
| Screencast | To be recorded and submitted by Daan | Pending |

The C# runner ended with `18 checks passed.` The HTTP runner printed `PASS` for each of S01 through S08 and exited 0. These are 26 named checks, some containing several assertions; they are not a coverage percentage.

## Reproduction and environment limitations

```bash
dotnet run --project tests/FootballClub.Tests.csproj
dotnet publish src/server/FootballClub.Server.csproj -c Release -o /tmp/football-publish
python3 scripts/system-tests.py
docker compose config --quiet
```

In this restricted environment, .NET commands used `DOTNET_CLI_HOME=/tmp/football-dotnet` and the existing package cache at `/home/daan/.nuget/packages`. Restore used `--ignore-failed-sources`; publish used `--no-restore` after restore completed. NuGet emitted NU1900 because the vulnerability-data endpoint could not be reached. Dependency vulnerability auditing therefore did not complete. HTTP system tests required permitted localhost socket access; after that they completed successfully.

## Findings and changes

Inspection found that the planner excluded field conflicts but allowed the same team to play simultaneously on a second field. The query now excludes overlapping matches and training sessions for either the selected team or field. I07–I09 verify these cases against the real SQL implementation.

The publish configuration now explicitly copies `Data/schema.sql`; published-process startup in S01–S02 verifies schema creation outside the source tree. A configurable database path makes isolated tests and persistent container storage possible. Unit tests were added for extracted overdue, player-threshold and opponent-normalization rules. Before this update, the repository contained no automated unit/integration suite.

## Assessment and remaining scope

All executed functional checks passed. This supports the tested sequential prototype workflows, not a production-readiness claim. HTTP checks do not validate browser rendering or accessibility. Concurrent planning can still race because reservation selection and insertion are not transactionally serialized. Authentication, actual email, malformed membership inputs, missing payment/member IDs, load testing and full security assessment remain outside this suite. Docker runtime verification and manual acceptance must still be completed; the published-process results do not establish that a container was deployed on a server.
