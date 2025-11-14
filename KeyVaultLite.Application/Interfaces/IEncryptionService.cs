namespace KeyVaultLite.Application.Interfaces;

public interface IEncryptionService
{
    (byte[] encryptedValue, byte[] iv) Encrypt(string plaintext);
    string Decrypt(byte[] encryptedValue, byte[] iv);
}

