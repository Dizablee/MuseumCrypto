namespace MuseumCrypto.Models;

public sealed class User
{
    public string UserId { get; set; } = "";
    public string Login { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsActive { get; set; }
    public RoleType Role { get; set; }
    public DateTime CreatedAt { get; set; }
}