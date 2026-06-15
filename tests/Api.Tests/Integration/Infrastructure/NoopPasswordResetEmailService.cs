using System.Threading.Tasks;
using Api.Interfaces.Security.Passwords;

namespace Api.Tests.Integration.Infrastructure;

public class NoopPasswordResetEmailService : IPasswordResetEmailService
{
    public Task SendEmailAsync(string toEmail, string resetLink)
    {
        // no-op for tests
        return Task.CompletedTask;
    }
}
