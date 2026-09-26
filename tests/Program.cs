using FootballClub.Server.Data;
using FootballClub.Server.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Data.Sqlite;

var root = Path.Combine(Path.GetTempPath(), "football-tests-" + Guid.NewGuid());
Directory.CreateDirectory(Path.Combine(root, "Data"));
File.Copy(Path.Combine(Directory.GetCurrentDirectory(), "src/server/Data/schema.sql"), Path.Combine(root, "Data/schema.sql"));
var count = 0;
void Check(string id, bool passed) { if (!passed) throw new Exception("FAIL " + id); Console.WriteLine("PASS " + id); count++; }
var today = DateOnly.FromDateTime(DateTime.Today);
try
{
    Check("U01 overdue yesterday", ClubRules.IsOverdue(today.AddDays(-1), null, today));
    Check("U02 due today", !ClubRules.IsOverdue(today, null, today));
    Check("U03 paid overdue", !ClubRules.IsOverdue(today.AddDays(-1), today, today));
    Check("U04 future due", !ClubRules.IsOverdue(today.AddDays(1), null, today));
    Check("U05 player threshold", ClubRules.HasEnoughPlayers(7, 7) && !ClubRules.HasEnoughPlayers(6, 7) && ClubRules.HasEnoughPlayers(8, 7));
    Check("U06 opponent normalization", ClubRules.NormalizeOpponent("  ") == "TBD" && ClubRules.NormalizeOpponent(" FC ") == "FC");
    var db = new ClubDatabase(new TestEnvironment { ContentRootPath = root }, new ConfigurationBuilder().Build());
    await db.InitializeAsync(); await db.SeedAsync(); await db.SeedAsync();
    var state = await db.GetDashboardAsync();
    Check("I01 schema and idempotent seed", state.MemberCount == 16 && state.TeamCount == 2 && state.Contributions.Count == 16 && state.OverdueCount == 10);
    var payment = state.Contributions.First(x => x.PaidDate is null);
    await db.RecordPaymentAsync(payment.Id); await db.RecordPaymentAsync(payment.Id);
    Check("I02 payment persistence and repeat", (await db.GetDashboardAsync()).OverdueCount == 9);
    await db.UpdateMembershipAsync(1, null);
    Check("I03 unassign membership", (await db.GetDashboardAsync()).Members.Single(x => x.Id == 1).TeamId is null);
    await db.UpdateMembershipAsync(1, 1);
    Check("I04 assign membership", (await db.GetDashboardAsync()).Members.Single(x => x.Id == 1).TeamId == 1);
    Check("I05 reminders and daily deduplication", await db.CreateRemindersAsync() == 9 && await db.CreateRemindersAsync() == 0);
    Check("I06 unknown team", !(await db.PlanMatchAsync(999, "FC")).Success);
    var first = await db.PlanMatchAsync(1, "FC 'Test'");
    var second = await db.PlanMatchAsync(1, "FC");
    Check("I07 planning and team conflict", first.Success && second.Success && first.Match!.StartsAt != second.Match!.StartsAt);
    await using var connection = new SqliteConnection($"Data Source={root}/football-club.db;Foreign Keys=True");
    await connection.OpenAsync();
    async Task Sql(string sql) { await using var command = connection.CreateCommand(); command.CommandText = sql; await command.ExecuteNonQueryAsync(); }
    await Sql("DELETE FROM matches; UPDATE availability SET available=1; INSERT INTO trainings(team_id,field_id,starts_at,ends_at) VALUES (1,1,'" + today.AddDays(1).ToString("yyyy-MM-dd") + "T18:30:00','" + today.AddDays(1).ToString("yyyy-MM-dd") + "T20:00:00');");
    var trainingConflict = await db.PlanMatchAsync(1, "FC");
    Check("I08 team training conflict", trainingConflict.Success && DateOnly.FromDateTime(trainingConflict.Match!.StartsAt) == today.AddDays(2));
    var otherTeam = await db.PlanMatchAsync(2, "FC");
    Check("I09 field training conflict", otherTeam.Success && otherTeam.Match!.FieldId == 2 && DateOnly.FromDateTime(otherTeam.Match.StartsAt) == today.AddDays(1));
    await Sql("UPDATE availability SET available=0");
    Check("I10 insufficient players", !(await db.PlanMatchAsync(1, "FC")).Success);
    var rejected = false;
    try { await db.UpdateMembershipAsync(1, 999); } catch (SqliteException) { rejected = true; }
    Check("I11 foreign key constraint", rejected);
    Check("I12 reopening preserves payment", (await new ClubDatabase(new TestEnvironment { ContentRootPath = root }, new ConfigurationBuilder().Build()).GetDashboardAsync()).OverdueCount == 9);
    Console.WriteLine($"{count} checks passed.");
}
finally { SqliteConnection.ClearAllPools(); Directory.Delete(root, true); }

sealed class TestEnvironment : IWebHostEnvironment
{
    public string ApplicationName { get; set; } = "Tests";
    public string EnvironmentName { get; set; } = "Testing";
    public string ContentRootPath { get; set; } = "";
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = "";
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
}
