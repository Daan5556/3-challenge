using System.Net;
using System.Text;
using FootballClub.Server.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<ClubDatabase>();
var app = builder.Build();

var database = app.Services.GetRequiredService<ClubDatabase>();
await database.InitializeAsync();
await database.SeedAsync();
app.UseStaticFiles();

app.MapGet("/", async (ClubDatabase db, string? message) =>
{
    var model = await db.GetDashboardAsync();
    static string H(object? value) => WebUtility.HtmlEncode(value?.ToString() ?? "");
    var html = new StringBuilder("""
        <!doctype html><html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
        <title>FC Admin</title><link rel="stylesheet" href="/styles.css"></head><body>
        <header><div><span class="eyebrow">CLUB CONTROL</span><h1>Football administration</h1><p>Members, contributions and planning in one clear overview.</p></div><div class="ball">⚽</div></header><main>
        """);
    if (!string.IsNullOrWhiteSpace(message)) html.Append($"<div class=notice role=status>{H(message)}</div>");
    html.Append($"""
        <section class=stats aria-label="Club statistics">
          <article><b>{model.MemberCount}</b><span>Members</span></article><article><b>{model.TeamCount}</b><span>Teams</span></article>
          <article class=warning><b>{model.OverdueCount}</b><span>Overdue</span></article><article><b>{model.UpcomingMatchCount}</b><span>Matches</span></article>
        </section>
        <div class=grid><section class=panel><span class=eyebrow>SMART PLANNER</span><h2>Schedule a match</h2>
        <p class=muted>The planner checks player availability and field conflicts for the next 14 days.</p>
        <form method=post action=/matches/plan><label>Team<select name=teamId required>
        """);
    foreach (var team in model.Teams) html.Append($"<option value={team.Id}>{H(team.Name)} (min. {team.MinimumPlayers})</option>");
    html.Append("""
        </select></label><label>Opponent<input name=opponent value="FC Rivals" required maxlength=80></label><button>Find and reserve a slot</button></form></section>
        <section class=panel><span class=eyebrow>ACTIONS</span><h2>Payment reminders</h2>
        <p class=muted>Create one reminder per overdue contribution per day. In this prototype, messages are recorded rather than emailed.</p>
        <form method=post action=/reminders><button class=secondary>Generate reminders</button></form>
        """);
    if (model.Reminders.Count > 0) { html.Append("<ul class=reminders>"); foreach (var reminder in model.Reminders) html.Append($"<li>{H(reminder)}</li>"); html.Append("</ul>"); }
    html.Append("</section></div>");
    html.Append("<section class=panel><span class=eyebrow>UPCOMING</span><h2>Matches</h2><div class=table-wrap><table><thead><tr><th>Date</th><th>Team</th><th>Opponent</th><th>Field</th><th>Status</th></tr></thead><tbody>");
    if (model.Matches.Count == 0) html.Append("<tr><td colspan=5 class=empty>No matches scheduled yet.</td></tr>");
    foreach (var match in model.Matches) html.Append($"<tr><td>{match.StartsAt:ddd d MMM, HH:mm}</td><td>{H(match.TeamName)}</td><td>{H(match.Opponent)}</td><td>{H(match.FieldName)}</td><td><span class=tag>{H(match.Status)}</span></td></tr>");
    html.Append("</tbody></table></div></section>");
    html.Append("<div class=grid><section class=panel><span class=eyebrow>FINANCE</span><h2>Contributions</h2><div class=table-wrap><table><thead><tr><th>Member</th><th>Amount</th><th>Due</th><th>Status</th></tr></thead><tbody>");
    foreach (var item in model.Contributions)
    {
        html.Append($"<tr><td>{H(item.MemberName)}</td><td>€{item.Amount:0.00}</td><td>{item.DueDate:dd-MM-yyyy}</td><td>");
        if (item.PaidDate is null) html.Append($"<form class=inline method=post action=/payments/{item.Id}><button class=small>Mark paid</button></form>"); else html.Append("<span class='tag success'>Paid</span>");
        html.Append("</td></tr>");
    }
    html.Append("</tbody></table></div></section>");
    html.Append("<section class=panel><span class=eyebrow>SQUADS</span><h2>Memberships</h2><div class=table-wrap><table><thead><tr><th>Member</th><th>Team</th></tr></thead><tbody>");
    foreach (var member in model.Members)
    {
        html.Append($"<tr><td>{H(member.Name)}</td><td><form class=membership method=post action=/memberships/{member.Id}><select name=teamId><option value=''>No team</option>");
        foreach (var team in model.Teams) html.Append($"<option value={team.Id} {(member.TeamId == team.Id ? "selected" : "")}>{H(team.Name)}</option>");
        html.Append("</select><button class=small>Save</button></form></td></tr>");
    }
    html.Append("</tbody></table></div></section></div></main><footer>FC Admin prototype · Direct SQL · ASP.NET Core</footer></body></html>");
    return Results.Content(html.ToString(), "text/html; charset=utf-8");
});

app.MapPost("/payments/{id:int}", async (int id, ClubDatabase db) => { await db.RecordPaymentAsync(id); return Results.Redirect("/?message=" + Uri.EscapeDataString("Payment registered.")); });
app.MapPost("/memberships/{id:int}", async (int id, HttpRequest request, ClubDatabase db) =>
{
    var form = await request.ReadFormAsync();
    int? teamId = int.TryParse(form["teamId"], out var parsed) ? parsed : null;
    await db.UpdateMembershipAsync(id, teamId);
    return Results.Redirect("/?message=" + Uri.EscapeDataString("Membership updated."));
});
app.MapPost("/reminders", async (ClubDatabase db) => { var count = await db.CreateRemindersAsync(); return Results.Redirect("/?message=" + Uri.EscapeDataString($"{count} reminder(s) generated.")); });
app.MapPost("/matches/plan", async (HttpRequest request, ClubDatabase db) =>
{
    var form = await request.ReadFormAsync();
    if (!int.TryParse(form["teamId"], out var teamId)) return Results.BadRequest("A valid team is required.");
    var result = await db.PlanMatchAsync(teamId, form["opponent"].ToString());
    return Results.Redirect("/?message=" + Uri.EscapeDataString(result.Message));
});
app.MapGet("/api/status", async (ClubDatabase db) => Results.Ok(await db.GetDashboardAsync()));
app.Run();

public partial class Program;
