using MuseumCrypto.Models;

namespace MuseumCrypto.Data;

public sealed class AuditRepository
{
    public void Add(AuditLogEntry e)
    {
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
@"INSERT INTO AuditLogs(LogId, UserId, Operation, FileName, Timestamp, Status, Details)
  VALUES ($id, $uid, $op, $fn, $ts, $st, $dt);";
        cmd.Parameters.AddWithValue("$id", e.LogId);
        cmd.Parameters.AddWithValue("$uid", e.UserId);
        cmd.Parameters.AddWithValue("$op", e.Operation.ToString());
        cmd.Parameters.AddWithValue("$fn", e.FileName);
        cmd.Parameters.AddWithValue("$ts", e.Timestamp);
        cmd.Parameters.AddWithValue("$st", e.Status.ToString());
        cmd.Parameters.AddWithValue("$dt", e.Details);
        cmd.ExecuteNonQuery();
    }

    public List<AuditLogEntry> GetAll()
    {
        var list = new List<AuditLogEntry>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
@"
SELECT a.LogId, a.UserId, u.Login, a.Operation, a.FileName, a.Timestamp, a.Status, IFNULL(a.Details,'')
FROM AuditLogs a
JOIN Users u ON u.UserId = a.UserId
ORDER BY a.Timestamp DESC;";
        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new AuditLogEntry
            {
                LogId = rd.GetString(0),
                UserId = rd.GetString(1),
                UserLogin = rd.GetString(2),
                Operation = Enum.Parse<CryptoOperationType>(rd.GetString(3), true),
                FileName = rd.GetString(4),
                Timestamp = rd.GetDateTime(5),
                Status = Enum.Parse<OperationStatus>(rd.GetString(6), true),
                Details = rd.GetString(7)
            });
        }
        return list;
    }
    public List<AuditLogEntry> GetByUser(string userId)
    {
        var list = new List<AuditLogEntry>();
        using var conn = Db.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
    @"
SELECT a.LogId, a.UserId, u.Login, a.Operation, a.FileName, a.Timestamp, a.Status, IFNULL(a.Details,'')
FROM AuditLogs a
JOIN Users u ON u.UserId = a.UserId
WHERE a.UserId = $uid
ORDER BY a.Timestamp DESC;";
        cmd.Parameters.AddWithValue("$uid", userId);

        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new AuditLogEntry
            {
                LogId = rd.GetString(0),
                UserId = rd.GetString(1),
                UserLogin = rd.GetString(2),
                Operation = Enum.Parse<CryptoOperationType>(rd.GetString(3), true),
                FileName = rd.GetString(4),
                Timestamp = rd.GetDateTime(5),
                Status = Enum.Parse<OperationStatus>(rd.GetString(6), true),
                Details = rd.GetString(7)
            });
        }
        return list;
    }
}