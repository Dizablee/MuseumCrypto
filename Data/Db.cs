using Microsoft.Data.Sqlite;

namespace MuseumCrypto.Data;

public static class Db
{
    public static readonly string DbPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.db");

    public static SqliteConnection Open()
    {
        var cs = new SqliteConnectionStringBuilder { DataSource = DbPath }.ToString();
        var conn = new SqliteConnection(cs);
        conn.Open();
        return conn;
    }

    public static void EnsureCreated()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText =
@"
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS Roles (
  RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
  Name   VARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS Users (
  UserId       TEXT PRIMARY KEY,
  Login        VARCHAR(50)  NOT NULL UNIQUE,
  PasswordHash VARCHAR(256) NOT NULL,
  IsActive     INTEGER      NOT NULL DEFAULT 1,
  RoleId       INTEGER      NOT NULL,
  CreatedAt    DATETIME     NOT NULL,
  FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

CREATE TABLE IF NOT EXISTS AuditLogs (
  LogId     TEXT PRIMARY KEY,
  UserId    TEXT         NOT NULL,
  Operation VARCHAR(20)  NOT NULL,
  FileName  VARCHAR(255) NOT NULL,
  Timestamp DATETIME     NOT NULL,
  Status    VARCHAR(10)  NOT NULL,
  Details   VARCHAR(500),
  FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

CREATE INDEX IF NOT EXISTS idx_audit_user_time ON AuditLogs(UserId, Timestamp);
CREATE INDEX IF NOT EXISTS idx_audit_time ON AuditLogs(Timestamp);
";
        cmd.ExecuteNonQuery();

        SeedRolesAndAdmin(conn);
    }

    private static void SeedRolesAndAdmin(SqliteConnection conn)
    {
        // роли
        Execute(conn, "INSERT OR IGNORE INTO Roles(Name) VALUES ('User');");
        Execute(conn, "INSERT OR IGNORE INTO Roles(Name) VALUES ('Admin');");
        Execute(conn, "INSERT OR IGNORE INTO Roles(Name) VALUES ('Manager');");

        // админ по умолчанию: admin / Admin123!
        // создаём только если нет ни одного пользователя
        using var check = conn.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM Users;";
        var count = Convert.ToInt32(check.ExecuteScalar());
        if (count > 0) return;

        var adminRoleId = GetRoleId(conn, "Admin");
        var userId = Guid.NewGuid().ToString();
        var login = "admin";
        var password = "Admin123!";

        var passHash = Security.PasswordHasher.HashPassword(password);

        using var ins = conn.CreateCommand();
        ins.CommandText =
@"INSERT INTO Users(UserId, Login, PasswordHash, IsActive, RoleId, CreatedAt)
  VALUES ($id, $login, $ph, 1, $roleId, $createdAt);";
        ins.Parameters.AddWithValue("$id", userId);
        ins.Parameters.AddWithValue("$login", login);
        ins.Parameters.AddWithValue("$ph", passHash);
        ins.Parameters.AddWithValue("$roleId", adminRoleId);
        ins.Parameters.AddWithValue("$createdAt", DateTime.UtcNow);
        ins.ExecuteNonQuery();
    }

    private static int GetRoleId(SqliteConnection conn, string name)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT RoleId FROM Roles WHERE Name = $n;";
        cmd.Parameters.AddWithValue("$n", name);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static void Execute(SqliteConnection conn, string sql)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }
}