using MuseumCrypto.Crypto;
using MuseumCrypto.Models;

namespace MuseumCrypto.Services;

public sealed class CryptoService
{
    public void Encrypt(string inputPath, string outputPath, string passphrase)
        => FileCrypto.EncryptFile(inputPath, outputPath, passphrase);

    public void Decrypt(string inputPath, string outputPath, string passphrase)
        => FileCrypto.DecryptFile(inputPath, outputPath, passphrase);
}