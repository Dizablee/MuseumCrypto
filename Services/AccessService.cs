using MuseumCrypto.Models;

namespace MuseumCrypto.Services;

public sealed class AccessService
{
    public bool CanEncrypt(User u) => u.Role is RoleType.User or RoleType.Admin or RoleType.Manager;
    public bool CanDecrypt(User u) => u.Role is RoleType.User or RoleType.Admin or RoleType.Manager;

    public bool CanViewLogs(User u) => u.Role is RoleType.Admin or RoleType.Manager;
    public bool CanManageUsers(User u) => u.Role is RoleType.Admin;
}