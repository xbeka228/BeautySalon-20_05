using Microsoft.Data.Sqlite;

namespace BeautySalon;

public sealed class SalonDataStore
{
    private readonly string _connectionString;

    private static readonly ServiceItem[] DefaultServices =
    [
        new("Стрижка и укладка", "Форма, уход и финальная укладка под ваш тип волос.", "60 мин", "от 8 000 ₸", "✂"),
        new("Окрашивание", "Тонирование, airtouch, однотонное окрашивание и уход.", "120 мин", "от 22 000 ₸", "◆"),
        new("Маникюр", "Комбинированный маникюр, покрытие и дизайн.", "90 мин", "от 7 500 ₸", "●"),
        new("Макияж", "Дневной, вечерний или свадебный образ.", "75 мин", "от 12 000 ₸", "◐")
    ];

    private static readonly MasterItem[] DefaultMasters =
    [
        new("Алина Ким", "Стилист-колорист", "9 лет опыта", "АК"),
        new("Мадина Омар", "Nail-мастер", "6 лет опыта", "МО"),
        new("Елена Нур", "Визажист", "8 лет опыта", "ЕН")
    ];

    public SalonDataStore(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var databasePath = configuration["Database:Path"] ?? "beauty-salon.db";
        if (!Path.IsPathRooted(databasePath))
        {
            databasePath = Path.Combine(environment.ContentRootPath, databasePath);
        }

        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath
        }.ToString();

        InitializeDatabase();
    }

    public IReadOnlyList<ServiceItem> GetServices()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Name, Description, Duration, Price, Icon
            FROM Services
            ORDER BY Id;
            """;

        using var reader = command.ExecuteReader();
        var services = new List<ServiceItem>();
        while (reader.Read())
        {
            services.Add(new ServiceItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)));
        }

        return services;
    }

    public IReadOnlyList<MasterItem> GetMasters()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Name, Role, Experience, Initials
            FROM Masters
            ORDER BY Id;
            """;

        using var reader = command.ExecuteReader();
        var masters = new List<MasterItem>();
        while (reader.Read())
        {
            masters.Add(new MasterItem(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3)));
        }

        return masters;
    }

    public IReadOnlyList<BookingLead> GetBookingLeads()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT Id, Name, Phone, Service, BookingDate, Comment, CreatedAt, IsAgreed
            FROM BookingLeads
            ORDER BY CreatedAt DESC, Id DESC;
            """;

        using var reader = command.ExecuteReader();
        var leads = new List<BookingLead>();
        while (reader.Read())
        {
            leads.Add(new BookingLead(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                DateOnly.Parse(reader.GetString(4)),
                reader.IsDBNull(5) ? null : reader.GetString(5),
                DateTimeOffset.Parse(reader.GetString(6)),
                reader.GetInt32(7) == 1));
        }

        return leads;
    }

    public void AddService(ServiceItem service)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Services (Name, Description, Duration, Price, Icon)
            VALUES ($name, $description, $duration, $price, $icon);
            """;
        command.Parameters.AddWithValue("$name", service.Name);
        command.Parameters.AddWithValue("$description", service.Description);
        command.Parameters.AddWithValue("$duration", service.Duration);
        command.Parameters.AddWithValue("$price", service.Price);
        command.Parameters.AddWithValue("$icon", service.Icon);
        command.ExecuteNonQuery();
    }

    public void SetBookingLeadAgreed(int id, bool isAgreed)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE BookingLeads
            SET IsAgreed = $isAgreed
            WHERE Id = $id;
            """;
        command.Parameters.AddWithValue("$id", id);
        command.Parameters.AddWithValue("$isAgreed", isAgreed ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public void DeleteBookingLead(int id)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM BookingLeads WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        command.ExecuteNonQuery();
    }

    public void AddMaster(MasterItem master)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Masters (Name, Role, Experience, Initials)
            VALUES ($name, $role, $experience, $initials);
            """;
        command.Parameters.AddWithValue("$name", master.Name);
        command.Parameters.AddWithValue("$role", master.Role);
        command.Parameters.AddWithValue("$experience", master.Experience);
        command.Parameters.AddWithValue("$initials", master.Initials);
        command.ExecuteNonQuery();
    }

    public void AddBookingLead(BookingLead lead)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO BookingLeads (Name, Phone, Service, BookingDate, Comment, CreatedAt)
            VALUES ($name, $phone, $service, $bookingDate, $comment, $createdAt);
            """;
        command.Parameters.AddWithValue("$name", lead.Name);
        command.Parameters.AddWithValue("$phone", lead.Phone);
        command.Parameters.AddWithValue("$service", lead.Service);
        command.Parameters.AddWithValue("$bookingDate", lead.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$comment", (object?)lead.Comment ?? DBNull.Value);
        command.Parameters.AddWithValue("$createdAt", lead.CreatedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private void InitializeDatabase()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS Services (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Duration TEXT NOT NULL,
                Price TEXT NOT NULL,
                Icon TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Masters (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Role TEXT NOT NULL,
                Experience TEXT NOT NULL,
                Initials TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS BookingLeads (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Phone TEXT NOT NULL,
                Service TEXT NOT NULL,
                BookingDate TEXT NOT NULL,
                Comment TEXT NULL,
                CreatedAt TEXT NOT NULL,
                IsAgreed INTEGER NOT NULL DEFAULT 0
            );
            """;
        command.ExecuteNonQuery();
        EnsureBookingLeadColumns(connection);

        SeedServices(connection);
        SeedMasters(connection);
    }

    private static void EnsureBookingLeadColumns(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA table_info(BookingLeads);";

        using var reader = command.ExecuteReader();
        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read())
        {
            columns.Add(reader.GetString(1));
        }

        if (columns.Contains("IsAgreed"))
        {
            return;
        }

        using var alterCommand = connection.CreateCommand();
        alterCommand.CommandText = "ALTER TABLE BookingLeads ADD COLUMN IsAgreed INTEGER NOT NULL DEFAULT 0;";
        alterCommand.ExecuteNonQuery();
    }

    private static void SeedServices(SqliteConnection connection)
    {
        using var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM Services;";
        var count = (long)countCommand.ExecuteScalar()!;
        if (count > 0)
        {
            return;
        }

        foreach (var service in DefaultServices)
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Services (Name, Description, Duration, Price, Icon)
                VALUES ($name, $description, $duration, $price, $icon);
                """;
            command.Parameters.AddWithValue("$name", service.Name);
            command.Parameters.AddWithValue("$description", service.Description);
            command.Parameters.AddWithValue("$duration", service.Duration);
            command.Parameters.AddWithValue("$price", service.Price);
            command.Parameters.AddWithValue("$icon", service.Icon);
            command.ExecuteNonQuery();
        }
    }

    private static void SeedMasters(SqliteConnection connection)
    {
        using var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM Masters;";
        var count = (long)countCommand.ExecuteScalar()!;
        if (count > 0)
        {
            return;
        }

        foreach (var master in DefaultMasters)
        {
            using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Masters (Name, Role, Experience, Initials)
                VALUES ($name, $role, $experience, $initials);
                """;
            command.Parameters.AddWithValue("$name", master.Name);
            command.Parameters.AddWithValue("$role", master.Role);
            command.Parameters.AddWithValue("$experience", master.Experience);
            command.Parameters.AddWithValue("$initials", master.Initials);
            command.ExecuteNonQuery();
        }
    }
}

public sealed record ServiceItem(string Name, string Description, string Duration, string Price, string Icon);

public sealed record MasterItem(string Name, string Role, string Experience, string Initials);

public sealed record BookingLead(
    int Id,
    string Name,
    string Phone,
    string Service,
    DateOnly Date,
    string? Comment,
    DateTimeOffset CreatedAt,
    bool IsAgreed);
