namespace Api.Helpers
{
    public static class LogActionDescribe
    {
        public static string Create(string entity, int id)
            => $"create {entity} id: {id}";

        public static string Update(string entity, int id)
            => $"update {entity} id: {id}";

        public static string Delete(string entity, int id)
            => $"delete {entity} id: {id}";

        public static string Login(string username)
            => $"user {username} fez login no sistema";

        public static string ExternalLogin(string username)
            => $"user {username} fez login por redirecionamento no sistema";

        public static string NewPasswordRequest(string username)
            => $"user {username} solicitou reset de senha";

        public static string PasswordReset(string username)
            => $"user {username} alterou a senha";
    }
}
