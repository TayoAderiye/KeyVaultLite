namespace KeyVaultLite.Application.Interfaces;

public interface IEncryptionService
{
    byte[] GenerateKey();
    (byte[] encryptedValue, byte[] iv) Encrypt(string plaintext, byte[] key);
    string Decrypt(byte[] encryptedValue, byte[] iv, byte[] key);
}

