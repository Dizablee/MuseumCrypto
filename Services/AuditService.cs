using MuseumCrypto.Data;
using MuseumCrypto.Models;

namespace MuseumCrypto.Services;

public sealed class AuditService
{
    private readonly AuditRepository _repo = new();

    public void Write(User user, CryptoOperationType op, string fileName, OperationStatus st, string details)
    {
        _repo.Add(new AuditLogEntry
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = user.UserId,
            UserLogin = user.Login,
            Operation = op,
            FileName = fileName,
            Timestamp = DateTime.UtcNow,
            Status = st,
            Details = details ?? ""
        });
    }

    public List<AuditLogEntry> GetAll() => _repo.GetAll();
    public List<AuditLogEntry> GetFor(User u, bool canViewAll)
    => canViewAll ? _repo.GetAll() : _repo.GetByUser(u.UserId);
}