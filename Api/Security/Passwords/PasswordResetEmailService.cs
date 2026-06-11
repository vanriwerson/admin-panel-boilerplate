using Api.Interfaces.Security.Passwords;
using Api.Settings;
using Microsoft.Extensions.Options;
using Resend;

namespace Api.Security.Passwords;

public class PasswordResetEmailService : IPasswordResetEmailService
{
    private readonly ResendClient _resendClient;
    private readonly ResendSettings _settings;

    public PasswordResetEmailService(ResendClient resendClient, IOptions<ResendSettings> settings)
    {
        _resendClient = resendClient;
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string resetLink)
    {
        var fromEmail = _settings.FromEmail;

        await _resendClient.EmailSendAsync(new EmailMessage
        {
            From = $"Suporte <{fromEmail}>",
            To = toEmail,
            Subject = "Redefinição de senha",
            HtmlBody = $@"
                    <h3>Você solicitou uma redefinição de senha</h3>
                    <p>Clique no link abaixo para criar uma nova senha:</p>
                    <p><a href='{resetLink}' target='_blank'>Redefinir minha senha</a></p>
                    <p>Esse link é válido por 15 minutos.</p>"
        });
    }
}

