using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FinalYearProject.Services.Encryption;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    public EncryptionService()
    {
        // 256-bit key for AES
        _key = new byte[32];

        var keyPath = "Keys/aes.key";

        Directory.CreateDirectory("Keys");

        if (!File.Exists(keyPath))
        {
            RandomNumberGenerator.Fill(_key);
            File.WriteAllBytes(keyPath, _key);
        }
        else
        {
            _key = File.ReadAllBytes(keyPath);
        }
    }

    public (string payload, string aesKey) Encrypt(string message)
    {
        byte[] plaintext = Encoding.UTF8.GetBytes(message);

        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];

        using (var aes = new AesGcm(_key))
        {
            aes.Encrypt(nonce, plaintext, ciphertext, tag);
        }

        var payload = new
        {
            nonce = Convert.ToBase64String(nonce),
            cipher = Convert.ToBase64String(ciphertext),
            tag = Convert.ToBase64String(tag)
        };

        return (
            JsonSerializer.Serialize(payload),
            Convert.ToBase64String(_key) // THIS is what Python needs
        );
    }

    public string Decrypt(string cipherJson)
    {
        var payload = JsonSerializer.Deserialize<dynamic>(cipherJson)!;

        byte[] nonce = Convert.FromBase64String((string)payload.GetProperty("nonce").GetString());
        byte[] cipher = Convert.FromBase64String((string)payload.GetProperty("cipher").GetString());
        byte[] tag = Convert.FromBase64String((string)payload.GetProperty("tag").GetString());

        byte[] plaintext = new byte[cipher.Length];

        using (var aes = new AesGcm(_key))
        {
            aes.Decrypt(nonce, cipher, tag, plaintext);
        }

        return Encoding.UTF8.GetString(plaintext);
    }
}