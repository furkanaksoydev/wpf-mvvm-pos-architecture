using Lavira.AkyaPOS.Core.Database;
using Lavira.AkyaPOS.Core.Models;
using Lavira.AkyaPOS.Core.Security;
using System.Data.SQLite;

namespace Lavira.AkyaPOS.Repositories
{
    public class UserRepository
    {
        public static bool IsAdminExists()
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand(
                "SELECT COUNT(1) FROM akya_users WHERE is_admin = 1",
                conn);

            var result = cmd.ExecuteScalar();
            return result != null && (long)result > 0;
        }

        public User GetByUsername(string username)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand(
                "SELECT * FROM akya_users WHERE username = @username",
                conn);

            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return null;

            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                IsAdmin = reader.GetInt32(3) == 1
            };
        }

        public static void CreateAdmin(string username, string password)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand(
                @"INSERT INTO akya_users (username, password_hash, is_admin)
                  VALUES (@u, @p, 1)", conn);

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", Core.Security.PasswordHasher.Hash(password));
            cmd.ExecuteNonQuery();
        }

        public static void CreateUser(string username, string password)
        {
            using var conn = new SQLiteConnection(DatabaseInitializer.ConnectionString);
            conn.Open();

            var cmd = new SQLiteCommand(
                @"INSERT INTO akya_users (username, password_hash, is_admin)
                  VALUES (@u, @p, 0)", conn);

            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", Core.Security.PasswordHasher.Hash(password));
            cmd.ExecuteNonQuery();
        }

        public bool ValidateAdmin(string plainPassword)
        {
            using var conn = new System.Data.SQLite.SQLiteConnection(Lavira.AkyaPOS.Core.Database.DatabaseInitializer.ConnectionString);
            conn.Open();

            var cmd = new System.Data.SQLite.SQLiteCommand(
                "SELECT password_hash FROM akya_users WHERE is_admin = 1", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string storedHash = reader.GetString(0);

                if (Core.Security.PasswordHasher.Verify(plainPassword, storedHash))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
