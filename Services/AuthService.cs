using MuseumCrypto.Data;
using MuseumCrypto.Models;
using MuseumCrypto.Security;

namespace MuseumCrypto.Services;

public sealed class AuthService
{
    private readonly UserRepository _users = new();

    public User? Login(string login, string password)
    {
        var user = _users.GetByLogin(login);
        if (user is null) return null;
        if (!user.IsActive) return null;
        if (!PasswordHasher.Verify(password, user.PasswordHash)) return null;
        return user;
    }
}