using Api.Helpers;
using Resend;
using System.Threading.Tasks;

namespace Api.Services
{
  public class EmailService
  {
    private readonly ResendClient _resendClient;

    public EmailService(ResendClient resendClient)
    {
      _resendClient = resendClient;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
      var fromEmail = EnvLoader.GetEnv("RESEND_FROM_EMAIL");

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
}
