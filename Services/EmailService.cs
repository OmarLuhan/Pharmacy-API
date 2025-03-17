using Farma_api.Dto.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace Farma_api.Services;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDto request);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task<bool> SendEmailAsync(EmailDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("MailSettings:UserName").Value));
        email.To.Add(MailboxAddress.Parse(request.For));
        email.Subject = request.Subject;
        email.Body = new TextPart(TextFormat.Html) { Text = request.Body };
        try
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config.GetSection("MailSettings:Host").Value,
                Convert.ToInt32(_config.GetSection("MailSettings:Port").Value),
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(
                _config["MailSettings:UserName"],
                _config["MailSettings:Password"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
            return true;
        }
        catch
        {
            return false;
        }
    }
}