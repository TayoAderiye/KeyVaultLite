using KeyVaultLite.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace KeyVaultLite.Application.Services;

public class EncryptionService : IEncryptionService
{
    //private readonly byte[] _masterKey;
    public byte[] GenerateKey()
    {
        var key = new byte[32]; // 256-bit key
        RandomNumberGenerator.Fill(key);
        return key;
    }
    public (byte[] encryptedValue, byte[] iv) Encrypt(string plaintext, byte[] key)
    {
        if (string.IsNullOrEmpty(plaintext))
            throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));

        if (key is not { Length: 32 })
            throw new ArgumentException("Key must be 32 bytes (256 bits)", nameof(key));

        var plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        var iv = new byte[12]; // 12 bytes (96 bits) for GCM
        RandomNumberGenerator.Fill(iv);
        
        var tag = new byte[16]; // 128-bit authentication tag
        var ciphertext = new byte[plaintextBytes.Length];
        
        using var aesGcm = new AesGcm(key);
        aesGcm.Encrypt(iv, plaintextBytes, ciphertext, tag);
        
        // Combine ciphertext + tag for storage
        var encryptedValue = new byte[ciphertext.Length + tag.Length];
        Buffer.BlockCopy(ciphertext, 0, encryptedValue, 0, ciphertext.Length);
        Buffer.BlockCopy(tag, 0, encryptedValue, ciphertext.Length, tag.Length);
        
        return (encryptedValue, iv);
    }

    public string Decrypt(byte[] encryptedValue, byte[] iv, byte[] key)
    {
        if (encryptedValue == null || encryptedValue.Length == 0)
            throw new ArgumentException("Encrypted value cannot be null or empty", nameof(encryptedValue));
        if (iv == null || iv.Length == 0)
            throw new ArgumentException("IV cannot be null or empty", nameof(iv));

        // Extract tag (last 16 bytes) and ciphertext
        var tag = new byte[16];
        var ciphertext = new byte[encryptedValue.Length - 16];
        
        Buffer.BlockCopy(encryptedValue, 0, ciphertext, 0, ciphertext.Length);
        Buffer.BlockCopy(encryptedValue, ciphertext.Length, tag, 0, tag.Length);
        
        var plaintext = new byte[ciphertext.Length];
        
        using var aesGcm = new AesGcm(key);
        aesGcm.Decrypt(iv, ciphertext, tag, plaintext);
        
        return Encoding.UTF8.GetString(plaintext);
    }
}

