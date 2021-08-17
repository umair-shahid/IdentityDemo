namespace IdentityDemo.Service.Interface
{
    public interface IRSAEncryptionService
    {
        string GetPublicKey();
        string Encrypt(string plainText);
        string Decrypt(string cypherText);
    }
}
