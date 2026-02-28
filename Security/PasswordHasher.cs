using System.Security.Cryptography;
using System.Text;

namespace MuseumCrypto.Security;

public static class PasswordHasher
{
    // формат хранения: base64(salt):base64(hash)
    public static string HashPassword(string password, int iterations = 100_000)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}:{iterations}";
    }

    public static bool Verify(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 3) return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] storedHash = Convert.FromBase64String(parts[1]);
        int iterations = int.TryParse(parts[2], out var it) ? it : 100_000;

        byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations,
            HashAlgorithmName.SHA256,
            storedHash.Length);

        return CryptographicOperations.FixedTimeEquals(storedHash, testHash);
    }
}