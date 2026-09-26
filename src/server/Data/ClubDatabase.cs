using FootballClub.Server.Models;
using Microsoft.Data.Sqlite;

namespace FootballClub.Server.Data;

public sealed class ClubDatabase(IWebHostEnvironment environment, IConfiguration configuration)
{
    private readonly string _connectionString = new SqliteConnectionStringBuilder
    {
        DataSource = configuration["Database:Path"] ?? Path.Combine(environment.ContentRootPath, "football-club.db"),
        ForeignKeys = true
    }.ToString();

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    public async Task InitializeAsync()
    {
        var schema = await File.ReadAllTextAsync(Path.Combine(environment.ContentRootPath, "Data", "schema.sql"));
        await using var connection = Open();
        await using var command = connection.CreateCommand();
        command.CommandText = schema;
        await command.ExecuteNonQueryAsync();
    }

    public async Task SeedAsync()
    {
        await using var connection = Open();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = connection.CreateCommand();
        count.Transaction = (SqliteTransaction)transaction;
        count.CommandText = "SELECT COUNT(*) FROM members";
        if (Convert.ToInt32(await count.ExecuteScalarAsync()) > 0) return;

        await ExecuteAsync(connection, transaction, "INSERT INTO teams(name, minimum_players) VALUES ('Falcons', 7), ('Lions', 7); INSERT INTO fields(name) VALUES ('Main field'), ('Training field');");
        string[] firstNames = ["Noah", "Liam", "Sem", "Finn", "Milan", "Lucas", "Daan", "Levi", "Sam", "Bram", "Mees", "Max", "Sara", "Lotte", "Emma", "Nora"];
        for (var i = 0; i < firstNames.Length; i++)
        {
            var member = connection.CreateCommand();
            member.Transaction = (SqliteTransaction)transaction;
            member.CommandText = "INSERT INTO members(name,email,team_id) VALUES ($name,$email,$team) RETURNING id";
            member.Parameters.AddWithValue("$name", $"{firstNames[i]} van FC Demo");
            member.Parameters.AddWithValue("$email", $"{firstNames[i].ToLowerInvariant()}{i + 1}@example.test");
            member.Parameters.AddWithValue("$team", i < 8 ? 1 : 2);
            var memberId = Convert.ToInt32(await member.ExecuteScalarAsync());

            var contribution = connection.CreateCommand();
            contribution.Transaction = (SqliteTransaction)transaction;
            contribution.CommandText = "INSERT INTO contributions(member_id,amount_cents,due_date,paid_date) VALUES ($member,12500,$due,$paid)";
            contribution.Parameters.AddWithValue("$member", memberId);
            contribution.Parameters.AddWithValue("$due", DateOnly.FromDateTime(DateTime.Today).AddDays(-14).ToString("yyyy-MM-dd"));
            contribution.Parameters.AddWithValue("$paid", i % 3 == 0 ? DateOnly.FromDateTime(DateTime.Today).AddDays(-7).ToString("yyyy-MM-dd") : DBNull.Value);
            await contribution.ExecuteNonQueryAsync();

            for (var day = 1; day <= 14; day++)
            {
                var availability = connection.CreateCommand();
                availability.Transaction = (SqliteTransaction)transaction;
                availability.CommandText = "INSERT INTO availability(member_id,available_date,available) VALUES ($member,$date,$available)";
                availability.Parameters.AddWithValue("$member", memberId);
                availability.Parameters.AddWithValue("$date", DateOnly.FromDateTime(DateTime.Today).AddDays(day).ToString("yyyy-MM-dd"));
                availability.Parameters.AddWithValue("$available", (i + day) % 5 == 0 ? 0 : 1);
                await availability.ExecuteNonQueryAsync();
            }
        }
        await transaction.CommitAsync();
    }

    private static async Task ExecuteAsync(SqliteConnection connection, System.Data.Common.DbTransaction transaction, string sql)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = (SqliteTransaction)transaction;
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Dashboard> GetDashboardAsync()
    {
        await using var connection = Open();
        var members = new List<Member>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT m.id,m.name,m.email,m.team_id,t.name FROM members m LEFT JOIN teams t ON t.id=m.team_id ORDER BY m.name";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) members.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetInt32(3), reader.IsDBNull(4) ? null : reader.GetString(4)));
        }
        var teams = new List<Team>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT id,name,minimum_players FROM teams ORDER BY name";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) teams.Add(new(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2)));
        }
        var contributions = new List<Contribution>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT c.id,c.member_id,m.name,c.amount_cents,c.due_date,c.paid_date FROM contributions c JOIN members m ON m.id=c.member_id ORDER BY c.due_date";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) contributions.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3) / 100m, DateOnly.Parse(reader.GetString(4)), reader.IsDBNull(5) ? null : DateOnly.Parse(reader.GetString(5))));
        }
        var matches = await GetMatchesAsync(connection);
        var reminders = new List<string>();
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT message FROM reminders ORDER BY sent_at DESC LIMIT 5";
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) reminders.Add(reader.GetString(0));
        }
        return new(members.Count, teams.Count, contributions.Count(x => ClubRules.IsOverdue(x.DueDate, x.PaidDate, DateOnly.FromDateTime(DateTime.Today))), matches.Count(x => x.StartsAt >= DateTime.Today), members, teams, contributions, matches, reminders);
    }

    public async Task RecordPaymentAsync(int contributionId)
    {
        await using var connection = Open();
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE contributions SET paid_date=$date WHERE id=$id AND paid_date IS NULL";
        command.Parameters.AddWithValue("$date", DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$id", contributionId);
        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateMembershipAsync(int memberId, int? teamId)
    {
        await using var connection = Open();
        await using var command = connection.CreateCommand();
        command.CommandText = "UPDATE members SET team_id=$team WHERE id=$id";
        command.Parameters.AddWithValue("$team", teamId is null ? DBNull.Value : teamId.Value);
        command.Parameters.AddWithValue("$id", memberId);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> CreateRemindersAsync()
    {
        await using var connection = Open();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT OR IGNORE INTO reminders(contribution_id,sent_at,message)
            SELECT c.id,$sent,'Payment reminder for ' || m.name || ': €' || printf('%.2f',c.amount_cents / 100.0) || ' was due on ' || c.due_date
            FROM contributions c JOIN members m ON m.id=c.member_id
            WHERE c.paid_date IS NULL AND c.due_date < $today
            """;
        command.Parameters.AddWithValue("$sent", DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$today", DateOnly.FromDateTime(DateTime.Today).ToString("yyyy-MM-dd"));
        return await command.ExecuteNonQueryAsync();
    }

    public async Task<PlanResult> PlanMatchAsync(int teamId, string opponent)
    {
        await using var connection = Open();
        var teamCommand = connection.CreateCommand();
        teamCommand.CommandText = "SELECT name,minimum_players FROM teams WHERE id=$id";
        teamCommand.Parameters.AddWithValue("$id", teamId);
        await using var teamReader = await teamCommand.ExecuteReaderAsync();
        if (!await teamReader.ReadAsync()) return new(false, "Team not found.");
        var teamName = teamReader.GetString(0);
        var minimumPlayers = teamReader.GetInt32(1);
        await teamReader.DisposeAsync();

        for (var day = 1; day <= 14; day++)
        {
            var date = DateOnly.FromDateTime(DateTime.Today).AddDays(day);
            var starts = date.ToDateTime(new TimeOnly(18, 30));
            var ends = starts.AddMinutes(90);
            var fieldCommand = connection.CreateCommand();
            fieldCommand.CommandText = """
                SELECT f.id,f.name FROM fields f
                WHERE NOT EXISTS (SELECT 1 FROM matches m WHERE (m.field_id=f.id OR m.team_id=$team) AND m.starts_at < $end AND m.ends_at > $start)
                  AND NOT EXISTS (SELECT 1 FROM trainings t WHERE (t.field_id=f.id OR t.team_id=$team) AND t.starts_at < $end AND t.ends_at > $start)
                ORDER BY f.id LIMIT 1
                """;
            fieldCommand.Parameters.AddWithValue("$team", teamId);
            fieldCommand.Parameters.AddWithValue("$start", starts.ToString("s"));
            fieldCommand.Parameters.AddWithValue("$end", ends.ToString("s"));
            await using var fieldReader = await fieldCommand.ExecuteReaderAsync();
            if (!await fieldReader.ReadAsync()) continue;
            var fieldId = fieldReader.GetInt32(0);
            var fieldName = fieldReader.GetString(1);
            await fieldReader.DisposeAsync();

            var availableCommand = connection.CreateCommand();
            availableCommand.CommandText = "SELECT COUNT(*) FROM members m JOIN availability a ON a.member_id=m.id WHERE m.team_id=$team AND a.available_date=$date AND a.available=1";
            availableCommand.Parameters.AddWithValue("$team", teamId);
            availableCommand.Parameters.AddWithValue("$date", date.ToString("yyyy-MM-dd"));
            var available = Convert.ToInt32(await availableCommand.ExecuteScalarAsync());
            if (!ClubRules.HasEnoughPlayers(available, minimumPlayers)) continue;

            var insert = connection.CreateCommand();
            insert.CommandText = "INSERT INTO matches(team_id,field_id,starts_at,ends_at,opponent,status) VALUES ($team,$field,$start,$end,$opponent,'Planned') RETURNING id";
            insert.Parameters.AddWithValue("$team", teamId);
            insert.Parameters.AddWithValue("$field", fieldId);
            insert.Parameters.AddWithValue("$start", starts.ToString("s"));
            insert.Parameters.AddWithValue("$end", ends.ToString("s"));
            insert.Parameters.AddWithValue("$opponent", ClubRules.NormalizeOpponent(opponent));
            var id = Convert.ToInt32(await insert.ExecuteScalarAsync());
            var match = new ScheduledMatch(id, teamId, teamName, fieldId, fieldName, starts, ClubRules.NormalizeOpponent(opponent), "Planned");
            return new(true, $"Match planned with {available} available players.", match);
        }
        return new(false, "No slot in the next 14 days has both enough available players and a free field.");
    }

    private static async Task<List<ScheduledMatch>> GetMatchesAsync(SqliteConnection connection)
    {
        var result = new List<ScheduledMatch>();
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT m.id,m.team_id,t.name,m.field_id,f.name,m.starts_at,m.opponent,m.status FROM matches m JOIN teams t ON t.id=m.team_id JOIN fields f ON f.id=m.field_id ORDER BY m.starts_at";
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync()) result.Add(new(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3), reader.GetString(4), DateTime.Parse(reader.GetString(5)), reader.GetString(6), reader.GetString(7)));
        return result;
    }
}
