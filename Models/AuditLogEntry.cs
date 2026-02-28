namespace MuseumCrypto.Models;

public sealed class AuditLogEntry
{
    public string LogId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string UserLogin { get; set; } = "";
    public CryptoOperationType Operation { get; set; }
    public string FileName { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public OperationStatus Status { get; set; }
    public string Details { get; set; } = "";
}