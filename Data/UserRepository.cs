using Microsoft.Data.Sqlite;
using MuseumCrypto.Models;

namespace MuseumCrypto.Data;

public sealed class UserRepository
{
    public User? GetByLogin(string login)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
@"
SELECT u.UserId, u.Login, u.PasswordHash, u.IsActive, r.Name, u.CreatedAt
FROM Users u
JOIN Roles r ON r.RoleId = u.RoleId
WHERE u.Login = $login
LIMIT 1;";
        cmd.Parameters.AddWithValue("$login", login);

        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;

        return new User
        {
            UserId = r.GetString(0),
            Login = r.GetString(1),
            PasswordHash = r.GetString(2),
            IsActive = r.GetInt32(3) == 1,
            Role = Enum.Parse<RoleType>(r.GetString(4), ignoreCase: true),
            CreatedAt = r.GetDateTime(5)
        };
    }

    public List<User> GetAll()
    {
        var list = new List<User>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
@"
SELECT u.UserId, u.Login, u.PasswordHash, u.IsActive, r.Name, u.CreatedAt
FROM Users u
JOIN Roles r ON r.RoleId = u.RoleId
ORDER BY u.Login;";
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new User
            {
                UserId = rd.GetString(0),
                Login = rd.GetString(1),
                PasswordHash = rd.GetString(2),
                IsActive = rd.GetInt32(3) == 1,
                Role = Enum.Parse<RoleType>(rd.GetString(4), true),
                CreatedAt = rd.GetDateTime(5)
            });
        }
        return list;
    }

    public void Add(string login, string passwordHash, RoleType role)
    {
        using var conn = Db.Open();
        var roleId = GetRoleId(conn, role.ToString());

        using var cmd = conn.CreateCommand();
        cmd.CommandText =
@"INSERT INTO Users(UserId, Login, PasswordHash, IsActive, RoleId, CreatedAt)
  VALUES ($id, $login, $ph, 1, $roleId, $createdAt);";
        cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("$login", login);
        cmd.Parameters.AddWithValue("$ph", passwordHash);
        cmd.Parameters.AddWithValue("$roleId", roleId);
        cmd.Parameters.AddWithValue("$createdAt", DateTime.UtcNow);
        cmd.ExecuteNonQuery();
    }

    private static int GetRoleId(SqliteConnection conn, string roleName)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT RoleId FROM Roles WHERE Name = $n;";
        cmd.Parameters.AddWithValue("$n", roleName);
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
   
    
        public void SetActive(string userId, bool isActive)
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Users SET IsActive = $a WHERE UserId = $id;";
            cmd.Parameters.AddWithValue("$a", isActive ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }

        public void SetRole(string userId, RoleType role)
        {
            using var conn = Db.Open();
            var roleId = GetRoleId(conn, role.ToString());

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Users SET RoleId = $rid WHERE UserId = $id;";
            cmd.Parameters.AddWithValue("$rid", roleId);
            cmd.Parameters.AddWithValue("$id", userId);
            cmd.ExecuteNonQuery();
        }

        public bool ExistsLogin(string login)
        {
            using var conn = Db.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Login = $l;";
            cmd.Parameters.AddWithValue("$l", login);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }
    }