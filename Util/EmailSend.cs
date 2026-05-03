using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Mail;
using Util.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Util
{
    public class EmailSend : IEmailSend, IEmailSender
    {
            private readonly EmailSettings _emailSettings;
        // Package Microsoft.Extensions.Option zal je moeten installeren
        //Microsoft.Extensions.Options provides a strongly typed way of specifying and accessing settings using dependency injection and acts
        //as a bridge between configuration
             public EmailSend(IOptions<EmailSettings> emailSettings)
            {
                _emailSettings = emailSettings.Value;
            }
            public async Task SendEmailAsync(
            string email, string subject, string message)
            {
                var mail = new MailMessage(); // aanmaken van een mail-object
                mail.To.Add(new MailAddress(email));
                mail.From = new
                MailAddress("efrathib.ticketing@gmail.com"); // hier komt jullie Gmail-adres
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                try
                {
                    using (var smtp = new SmtpClient(_emailSettings.MailServer))
                    {
                        smtp.Port = _emailSettings.MailPort;
                        smtp.EnableSsl = true;
                        smtp.Credentials =
                        new NetworkCredential(_emailSettings.Sender,
                        _emailSettings.Password);
                        await smtp.SendMailAsync(mail);
                    }
                }
                catch (Exception ex)
                { throw ex; }
            }
        }
}
