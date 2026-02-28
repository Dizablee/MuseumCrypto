using System.Security.Cryptography;
using System.Text;

namespace MuseumCrypto.Crypto;

public static class FileCrypto
{
    private static readonly byte[] Magic = Encoding.ASCII.GetBytes("MCF1"); // Museum Crypto File v1

    public static void EncryptFile(string inputPath, string outputPath, string passphrase)
    {
        byte[] plain = File.ReadAllBytes(inputPath); // для курсовой ок
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] key = DeriveKey(passphrase, salt);
        byte[] nonce = RandomNumberGenerator.GetBytes(12);

        byte[] cipher = new byte[plain.Length];
        byte[] tag = new byte[16];

        using (var aes = new AesGcm(key))
        {
            aes.Encrypt(nonce, plain, cipher, tag);
        }

        using var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
        fs.Write(Magic);
        fs.WriteByte(1);          // version
        fs.Write(salt);
        fs.Write(nonce);
        fs.Write(tag);
        fs.Write(cipher);
    }

    public static void DecryptFile(string inputPath, string outputPath, string passphrase)
    {
        byte[] all = File.ReadAllBytes(inputPath);

        // header: 4 + 1 + 16 + 12 + 16 = 49
        if (all.Length < 49) throw new InvalidDataException("Неверный формат файла.");

        if (!all.Take(4).SequenceEqual(Magic))
            throw new InvalidDataException("Неверный формат (MAGIC).");

        byte version = all[4];
        if (version != 1) throw new InvalidDataException("Неподдерживаемая версия.");

        byte[] salt = all.Skip(5).Take(16).ToArray();
        byte[] nonce = all.Skip(21).Take(12).ToArray();
        byte[] tag = all.Skip(33).Take(16).ToArray();
        byte[] cipher = all.Skip(49).ToArray();

        byte[] key = DeriveKey(passphrase, salt);
        byte[] plain = new byte[cipher.Length];

        using (var aes = new AesGcm(key))
        {
            aes.Decrypt(nonce, cipher, tag, plain);
        }

        File.WriteAllBytes(outputPath, plain);
    }

    private static byte[] DeriveKey(string passphrase, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(passphrase),
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);
    }
}