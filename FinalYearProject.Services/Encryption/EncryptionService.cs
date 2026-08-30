using FinalYearProject.Data.Domain.Config;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FinalYearProject.Services.Encryption;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService(FileSystemConfig config)
    {
        // Base64 AES key from appsettings
        _key = Convert.FromBase64String(config.AESKey);
    }

    public string Encrypt(string plainText)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] ciphertext = new byte[plaintextBytes.Length];
        byte[] tag = new byte[16];

        using var aes = new AesGcm(_key);

        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        var payload = new
        {
            nonce = Convert.ToBase64String(nonce),
            cipher = Convert.ToBase64String(ciphertext),
            tag = Convert.ToBase64String(tag)
        };

        return JsonSerializer.Serialize(payload);
    }

    public string Decrypt(string cipherJson)
    {
        var payload = JsonSerializer.Deserialize<CipherPayload>(cipherJson)
            ?? throw new Exception("Invalid cipher payload");

        byte[] nonce = Convert.FromBase64String(payload.Nonce);
        byte[] cipher = Convert.FromBase64String(payload.Cipher);
        byte[] tag = Convert.FromBase64String(payload.Tag);

        byte[] plaintext = new byte[cipher.Length];

        using var aes = new AesGcm(_key);

        aes.Decrypt(nonce, cipher, tag, plaintext);

        return Encoding.UTF8.GetString(plaintext);
    }

    public EncryptedFile EncryptFile(byte[] fileBytes, byte[] aesKey)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(12);
        byte[] tag = new byte[16];
        byte[] cipher = new byte[fileBytes.Length];

        using var aes = new AesGcm(aesKey);

        aes.Encrypt(
            nonce,
            fileBytes,
            cipher,
            tag);

        return new EncryptedFile
        {
            Cipher = cipher,
            Nonce = nonce,
            Tag = tag
        };
    }

    public byte[] DecryptFile(
    EncryptedFile encrypted,
    byte[] aesKey)
    {
        byte[] plaintext = new byte[encrypted.Cipher.Length];

        using var aes = new AesGcm(aesKey);

        aes.Decrypt(
            encrypted.Nonce,
            encrypted.Cipher,
            encrypted.Tag,
            plaintext);

        return plaintext;
    }

    private byte[] GenerateFileKey()
    {
        return RandomNumberGenerator.GetBytes(32);
    }
}