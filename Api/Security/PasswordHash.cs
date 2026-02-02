using BCrypt.Net;

namespace Api.Helpers
{
    public static class PasswordHash
    {
        public static string Generate(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
