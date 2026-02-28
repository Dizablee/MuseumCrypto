using MuseumCrypto.Models;

namespace MuseumCrypto.Services;

public sealed class StorageService
{
    public string BuildOutputPath(string inputPath, CryptoOperationType op)
    {
        var dir = Path.GetDirectoryName(inputPath) ?? "";
        var name = Path.GetFileName(inputPath);

        return op == CryptoOperationType.Encrypt
            ? Path.Combine(dir, name + ".enc")
            : Path.Combine(dir, name.Replace(".enc", "") + ".dec");
    }
}