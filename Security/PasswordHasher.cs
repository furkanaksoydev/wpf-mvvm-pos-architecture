using System.Security.Cryptography;
using System.Text;

namespace Lavira.AkyaPOS.Core.Security
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha.ComputeHash(bytes);

            var sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
                return false;

            string hashedInput = Hash(password);

            return string.Equals(hashedInput, storedHash, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
