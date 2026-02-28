using MuseumCrypto.Data;
using MuseumCrypto.Models;
using MuseumCrypto.Security;

namespace MuseumCrypto.Services;

public sealed class UserManagementService
{
    private readonly UserRepository _users = new();

    public List<User> GetAll() => _users.GetAll();

    public void CreateUser(string login, string password, RoleType role)
    {
        login = (login ?? "").Trim();
        if (login.Length < 3) throw new ArgumentException("Логин слишком короткий.");
        if (_users.ExistsLogin(login)) throw new ArgumentException("Логин уже существует.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Пароль минимум 6 символов.");

        var hash = PasswordHasher.HashPassword(password);
        _users.Add(login, hash, role);
    }

    public void SetActive(string userId, bool isActive) => _users.SetActive(userId, isActive);

    public void SetRole(string userId, RoleType role) => _users.SetRole(userId, role);
}