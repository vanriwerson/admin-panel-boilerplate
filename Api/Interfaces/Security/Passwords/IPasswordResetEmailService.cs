namespace Api.Interfaces.Security.Passwords;

public interface IPasswordResetEmailService
{
    Task SendEmailAsync(string toEmail, string resetLink);
}