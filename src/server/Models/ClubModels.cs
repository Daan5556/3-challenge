namespace FootballClub.Server.Models;

public sealed record Team(int Id, string Name, int MinimumPlayers);
public sealed record Member(int Id, string Name, string Email, int? TeamId, string? TeamName);
public sealed record Contribution(int Id, int MemberId, string MemberName, decimal Amount, DateOnly DueDate, DateOnly? PaidDate);
public sealed record ScheduledMatch(int Id, int TeamId, string TeamName, int FieldId, string FieldName, DateTime StartsAt, string Opponent, string Status);
public sealed record Dashboard(int MemberCount, int TeamCount, int OverdueCount, int UpcomingMatchCount,
    IReadOnlyList<Member> Members, IReadOnlyList<Team> Teams, IReadOnlyList<Contribution> Contributions,
    IReadOnlyList<ScheduledMatch> Matches, IReadOnlyList<string> Reminders);
public sealed record PlanResult(bool Success, string Message, ScheduledMatch? Match = null);
