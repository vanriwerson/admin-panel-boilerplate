using BCrypt.Net;

namespace Api.Helpers
{
    public static class PasswordHashing
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
