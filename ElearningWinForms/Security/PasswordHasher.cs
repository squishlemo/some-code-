using System;
using System.Security.Cryptography;

namespace ElearningWinForms.Security
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;   // bytes
        private const int KeySize = 32;    // bytes
        private const int DefaultIterations = 150000;

        // Format: PBKDF2$V1$<iterations>$<saltB64>$<hashB64>
        public static string HashPassword(string password, int? iterationsOverride = null)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is empty.", nameof(password));

            int iterations = iterationsOverride ?? DefaultIterations;

            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] key = pbkdf2.GetBytes(KeySize);
                return $"PBKDF2$V1${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
            }
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            // Backward-compat: demo DB đang lưu plaintext '123456'
            if (!storedHash.StartsWith("PBKDF2$", StringComparison.Ordinal))
                return string.Equals(password, storedHash, StringComparison.Ordinal);

            string[] parts = storedHash.Split('$');
            if (parts.Length != 5) return false;
            if (!parts[0].Equals("PBKDF2", StringComparison.Ordinal)) return false;
            if (!parts[1].Equals("V1", StringComparison.Ordinal)) return false;

            if (!int.TryParse(parts[2], out int iterations)) return false;

            byte[] salt;
            byte[] expectedKey;
            try
            {
                salt = Convert.FromBase64String(parts[3]);
                expectedKey = Convert.FromBase64String(parts[4]);
            }
            catch
            {
                return false;
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] actualKey = pbkdf2.GetBytes(expectedKey.Length);
                return FixedTimeEquals(actualKey, expectedKey);
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
