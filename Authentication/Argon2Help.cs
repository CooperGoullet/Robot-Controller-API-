using System;
using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;

namespace FullImplementaionAPI.Authentication
{
    public class Argon2Help
    {
        private const int SaltSize = 16;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            var config = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = 1,
                MemoryCost = 8192, // 8 MB
                Lanes = 2,
                Threads = Environment.ProcessorCount,
                Salt = salt,
                Password = Encoding.UTF8.GetBytes(password),
                HashLength = 32
            };
            // Create Argon2 instance and hash
            using (var argon2 = new Argon2(config))
            {
                var hash = argon2.Hash();
                return config.EncodeString(hash.Buffer); // Provide hash as argument
            }
        }

        public static bool VerifyPassword(string password, string encodedHash)
        {
            return Argon2.Verify(encodedHash, password);
        }
    }
}
