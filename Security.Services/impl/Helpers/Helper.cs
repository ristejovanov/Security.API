using Security.DataServices.interfaces.Helpers;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Security.DataServices.impl.Helpers
{
    public class Helper : IHelper
    {
        private readonly int _iterations = 2;
        private readonly int _memoryKb = 1 << 16; 
        private readonly int _degreeOfParallelism = 2;

        private readonly int _length = 32;
        private readonly int _saltSize = 16;


        public (string PlainKey, byte[] Hash, byte[] Salt) GenerateApiKey()
        {
            var keyBytes = RandomNumberGenerator.GetBytes(_length);
            var plainKey = Convert.ToBase64String(keyBytes);

            var (hash, salt) = Hash(plainKey);
            return (plainKey, hash, salt);
        }

        public (byte[] hash, byte[] salt) Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_saltSize);
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Iterations = _iterations,
                MemorySize = _memoryKb,
                DegreeOfParallelism = _degreeOfParallelism
            };

            argon2.Salt = salt;
            var hash = argon2.GetBytes(_length);
            return (hash, salt);
        }

        public bool Verify(string password, byte[] salt, byte[] expectedHash)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Iterations = _iterations,
                MemorySize = _memoryKb,
                DegreeOfParallelism = _degreeOfParallelism
            };
            argon2.Salt = salt;
            var hash = argon2.GetBytes(_length);
            return CryptographicOperations.FixedTimeEquals(hash, expectedHash);
        }
    }
}

