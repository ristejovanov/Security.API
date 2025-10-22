namespace Security.DataServices.interfaces.Helpers
{
    public interface IHelper
    {
        (string PlainKey, byte[] Hash, byte[] Salt) GenerateApiKey();
        (byte[] hash, byte[] salt) Hash(string password);
        bool Verify(string password, byte[] salt, byte[] expectedHash);
    }
}
