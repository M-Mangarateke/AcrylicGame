using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace AcrylicGame.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Dummy email sent to {email} | Subject: {subject}");
            return Task.CompletedTask;
        }
    }
}
