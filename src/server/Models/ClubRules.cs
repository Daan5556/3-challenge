namespace FootballClub.Server.Models;

public static class ClubRules
{
    public static bool IsOverdue(DateOnly dueDate, DateOnly? paidDate, DateOnly today)
        => paidDate is null && dueDate < today;

    public static bool HasEnoughPlayers(int available, int minimumPlayers)
        => available >= minimumPlayers;

    public static string NormalizeOpponent(string opponent)
        => string.IsNullOrWhiteSpace(opponent) ? "TBD" : opponent.Trim();
}
