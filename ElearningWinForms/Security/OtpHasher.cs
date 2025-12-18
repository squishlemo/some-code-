using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace ElearningWinForms.Security
{
    public static class OtpHasher
    {
        // Generate numeric OTP using crypto-secure RNG.
        public static string GenerateNumericOtp(int digits = 6)
        {
            if (digits < 4 || digits > 10)
                throw new ArgumentOutOfRangeException(nameof(digits));

            int max = (int)Math.Pow(10, digits);
            int value;
            using (var rng = RandomNumberGenerator.Create())
            {
                // Generate a random int in [0, max)
                byte[] bytes = new byte[4];
                do
                {
                    rng.GetBytes(bytes);
                    value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
                } while (value >= max * (int.MaxValue / max));
                value = value % max;
            }
            return value.ToString(new string('0', digits));
        }

        // Compute HMAC-SHA256(pepper, "REG:<username>:<otp>")
        public static byte[] ComputeOtpHash(string username, string otp)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("username empty");
            if (string.IsNullOrWhiteSpace(otp)) throw new ArgumentException("otp empty");

            string pepper = GetPepper();
            byte[] key = Encoding.UTF8.GetBytes(pepper);
            byte[] msg = Encoding.UTF8.GetBytes($"REG:{username}:{otp}");

            using (var h = new HMACSHA256(key))
            {
                return h.ComputeHash(msg);
            }
        }

        public static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        private static string GetPepper()
        {
            // Ơi ưu tiên env var để tránh commit secret lên git.
            string env = Environment.GetEnvironmentVariable("ELEARNING_OTP_PEPPER");
            if (!string.IsNullOrWhiteSpace(env))
                return env;

            string cfg = ConfigurationManager.AppSettings["OtpPepper"];
            if (!string.IsNullOrWhiteSpace(cfg))
                return cfg;

            // Dev fallback (NHỚ đổi khi demo/bảo vệ)
            return "CHANGE_ME_OTP_PEPPER";
        }
    }
}
