using Api.Helpers;

namespace Api.Settings;

public static class SettingsFactory
{
    public static string ProvideDatabaseConnectionString()
    {
        return
            $"Host={EnvLoader.GetEnv("DB_HOST")};" +
            $"Port={EnvLoader.GetEnv("DB_PORT")};" +
            $"Username={EnvLoader.GetEnv("DB_USER")};" +
            $"Password={EnvLoader.GetEnv("DB_PASSWORD")};" +
            $"Database={EnvLoader.GetEnv("DB_NAME")}";
    }

    public static JwtSettings ProvideJwtSettings()
    {
        return new JwtSettings
        {
            SecretKey = EnvLoader.GetEnv("JWT_SECRET_KEY")
        };
    }

    public static ResendSettings ProvideResendSettings()
    {
        return new ResendSettings
        {
            ApiKey = EnvLoader.GetEnv("RESEND_API_KEY"),
            FromEmail = EnvLoader.GetEnv("RESEND_FROM_EMAIL")
        };
    }

    public static FrontendSettings ProvideFrontendSettings()
    {
        return new FrontendSettings
        {
            Url = EnvLoader.GetEnv("WEB_APP_URL")
        };
    }

    public static ApiSettings ProvideApiSettings()
    {
        return new ApiSettings
        {
            Port = EnvLoader.GetEnv("API_PORT")
        };
    }
}