using Domains.Entities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Util.Mail.Interfaces;

namespace Util.Mail
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

        public async Task SendOrderConfirmationEmailAsync(string email, string subject, Order order, byte[] ticketPdf, byte[] abonnementPdf)
        {
            var mail = new MailMessage();
            mail.To.Add(new MailAddress(email));
            mail.From = new MailAddress("EfraThib.ticketing@gmail.com");
            mail.Subject = subject;
            mail.Body = BuildOrderEmailHtml(order);
            mail.IsBodyHtml = true;

            if (ticketPdf?.Length > 0)
            {
                var ticketStream = new MemoryStream(ticketPdf);
                mail.Attachments.Add(new Attachment(ticketStream, "Tickets.pdf", MediaTypeNames.Application.Pdf));
            }

            if (abonnementPdf?.Length > 0)
            {
                var abonnementStream = new MemoryStream(abonnementPdf);
                mail.Attachments.Add(new Attachment(abonnementStream, "SeasonPasses.pdf", MediaTypeNames.Application.Pdf));
            }

            try
            {
                using var smtp = new SmtpClient(_emailSettings.MailServer);
                smtp.Port = _emailSettings.MailPort;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);
                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex) { throw ex; }
        }

        private static string BuildOrderEmailHtml(Order order)
        {
            string userName = order.User?.UserName ?? "there";
            string orderId = order.Id.ToString();
            string orderDate = order.OrderDate.ToString("d MMM yyyy · HH:mm");
            string total = order.TotalPrice.HasValue ? $"€{order.TotalPrice:0.##}" : "—";
            int year = DateTime.Now.Year;

            var sb = new StringBuilder();

            sb.Append($@"
<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width,initial-scale=1.0"" />
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"" />
  <title>Your EfraThib Ticketing Order</title>
</head>
<body style=""margin:0;padding:0;background-color:#f0f2f5;font-family:Arial,Helvetica,sans-serif;"">

<table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
       style=""background-color:#f0f2f5;padding:32px 16px;"">
  <tr><td align=""center"">

    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
           style=""max-width:600px;background-color:#ffffff;border-radius:12px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,0.08);"">

      <!-- Header -->
      <tr>
        <td style=""background-color:#0b1530;padding:28px 40px;border-bottom:3px solid #d4af37;"">
          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"">
            <tr>
              <td>
                <span style=""font-size:20px;font-weight:700;color:#ffffff;"">
                  EfraThib<span style=""color:#d4af37;"">_Ticketing</span>
                </span>
              </td>
              <td align=""right"">
                <span style=""font-size:11px;font-weight:600;letter-spacing:0.12em;text-transform:uppercase;color:#d4af37;"">
                  UEFA Champions League
                </span>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Hero -->
      <tr>
        <td style=""background-color:#112060;padding:40px 40px 32px;text-align:center;"">
          <div style=""display:inline-block;width:64px;height:64px;background-color:rgba(212,175,55,0.15);border:2px solid #d4af37;border-radius:50%;text-align:center;line-height:64px;font-size:28px;color:#d4af37;margin-bottom:20px;"">✓</div>
          <h1 style=""margin:0 0 12px;font-size:26px;font-weight:700;color:#ffffff;line-height:1.2;"">Your Order is Confirmed!</h1>
          <p style=""margin:0 auto;font-size:15px;color:#8fa3d0;line-height:1.6;max-width:420px;"">
            Hi {userName}, thank you for your purchase.<br />
            Your tickets are attached to this email as a PDF file.
          </p>
        </td>
      </tr>

      <!-- Order meta -->
      <tr>
        <td style=""padding:32px 40px 0;"">
          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
                 style=""background-color:#f7f8fa;border-radius:8px;border:1px solid #e4e6ea;"">
            <tr>
              <td style=""padding:16px 20px;"">
                <p style=""margin:0 0 3px;font-size:10px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#888;"">Order Reference</p>
                <p style=""margin:0;font-size:18px;font-weight:700;color:#0b1530;"">#{orderId}</p>
              </td>
              <td style=""padding:16px 20px;border-left:1px solid #e4e6ea;"">
                <p style=""margin:0 0 3px;font-size:10px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#888;"">Order Date</p>
                <p style=""margin:0;font-size:14px;font-weight:600;color:#0b1530;"">{orderDate}</p>
              </td>
              <td style=""padding:16px 20px;border-left:1px solid #e4e6ea;"">
                <p style=""margin:0 0 3px;font-size:10px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#888;"">Total Paid</p>
                <p style=""margin:0;font-size:18px;font-weight:700;color:#d4af37;"">{total}</p>
              </td>
            </tr>
          </table>
        </td>
      </tr>");

            // ── Match tickets ─────────────────────────────────────────
            if (order.Tickets?.Any() == true)
            {
                sb.Append($@"
      <tr>
        <td style=""padding:28px 40px 0;"">
          <p style=""margin:0 0 4px;font-size:10px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#888;"">Match Tickets ({order.Tickets.Count})</p>
          <p style=""margin:0 0 14px;font-size:16px;font-weight:700;color:#0b1530;"">Attached as PDF</p>");

                foreach (var ticket in order.Tickets)
                {
                    string homeTeam = ticket.Match?.HomeClub?.Name ?? "—";
                    string awayTeam = ticket.Match?.AwayClub?.Name ?? "—";
                    string matchLine = $"{homeTeam} vs {awayTeam}";
                    string stadium = ticket.Match?.HomeClub?.Stadium?.Name ?? "—";
                    string dateLine = $"{ticket.Match?.MatchDate:ddd d MMM yyyy · HH:mm} &nbsp;·&nbsp; {stadium}";
                    string vakName = ticket.Vak?.Name ?? "—";
                    string ring = ticket.Vak?.Ring;
                    string ringCell = !string.IsNullOrEmpty(ring)
                        ? $@"<td style=""padding-right:16px;"">
                       <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Ring</span>
                       <span style=""font-size:13px;font-weight:600;color:#0b1530;"">{ring}</span>
                     </td>"
                        : "";
                    string price = $"€{ticket.Vak?.Price:0.##}";

                    sb.Append($@"
          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
                 style=""background-color:#f7f8fa;border:1px solid #e4e6ea;border-left:4px solid #d4af37;border-radius:8px;margin-bottom:10px;"">
            <tr>
              <td style=""padding:14px 18px;"">
                <p style=""margin:0 0 3px;font-size:14px;font-weight:700;color:#0b1530;"">{matchLine}</p>
                <p style=""margin:0 0 10px;font-size:12px;color:#666;"">{dateLine}</p>
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                  <tr>
                    <td style=""padding-right:16px;"">
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Section</span>
                      <span style=""font-size:13px;font-weight:600;color:#0b1530;"">{vakName}</span>
                    </td>
                    {ringCell}
                    <td style=""padding-right:16px;"">
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Seat</span>
                      <span style=""font-size:13px;font-weight:700;color:#d4af37;"">{ticket.Seat}</span>
                    </td>
                    <td>
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Price</span>
                      <span style=""font-size:13px;font-weight:700;color:#0b1530;"">{price}</span>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>");
                }

                sb.Append("</td></tr>");
            }

            // ── Season passes ─────────────────────────────────────────
            if (order.Abonnements?.Any() == true)
            {
                sb.Append($@"
      <tr>
        <td style=""padding:28px 40px 0;"">
          <p style=""margin:0 0 4px;font-size:10px;font-weight:700;letter-spacing:0.1em;text-transform:uppercase;color:#888;"">Season Passes ({order.Abonnements.Count})</p>
          <p style=""margin:0 0 14px;font-size:16px;font-weight:700;color:#0b1530;"">Attached as PDF</p>");

                foreach (var sub in order.Abonnements)
                {
                    string clubName = sub.Club?.Name ?? "—";
                    string vakName = sub.Vak?.Name ?? "—";
                    string ring = !string.IsNullOrEmpty(sub.Vak?.Ring) ? sub.Vak.Ring : "—";
                    string seizoen = sub.Seizoen?.Name ?? "—";
                    string subPrice = $"€{sub.Vak?.SubscriptionPrice:0.##}";

                    sb.Append($@"
          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
                 style=""background-color:#f7f8fa;border:1px solid #e4e6ea;border-left:4px solid #112060;border-radius:8px;margin-bottom:10px;"">
            <tr>
              <td style=""padding:14px 18px;"">
                <p style=""margin:0 0 3px;font-size:14px;font-weight:700;color:#0b1530;"">{clubName} — Season Pass</p>
                <p style=""margin:0 0 10px;font-size:12px;color:#666;"">{seizoen}</p>
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"">
                  <tr>
                    <td style=""padding-right:16px;"">
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Section</span>
                      <span style=""font-size:13px;font-weight:600;color:#0b1530;"">{vakName}</span>
                    </td>
                    <td style=""padding-right:16px;"">
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Ring</span>
                      <span style=""font-size:13px;font-weight:600;color:#0b1530;"">{ring}</span>
                    </td>
                    <td style=""padding-right:16px;"">
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Seat</span>
                      <span style=""font-size:13px;font-weight:700;color:#d4af37;"">{sub.Seat}</span>
                    </td>
                    <td>
                      <span style=""font-size:10px;font-weight:700;letter-spacing:0.08em;text-transform:uppercase;color:#888;display:block;"">Price</span>
                      <span style=""font-size:13px;font-weight:700;color:#0b1530;"">{subPrice}</span>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>");
                }

                sb.Append("</td></tr>");
            }

            // ── PDF notice + footer ───────────────────────────────────
            sb.Append($@"
      <!-- PDF notice -->
      <tr>
        <td style=""padding:24px 40px 0;"">
          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
                 style=""background-color:#fffbea;border:1px solid #f0d060;border-radius:8px;"">
            <tr>
              <td style=""padding:16px 20px;"">
                <p style=""margin:0;font-size:13px;color:#7a6000;line-height:1.6;"">
                  <strong>Check your attachments.</strong>
                  Your tickets are included as a PDF file attached to this email.
                  Please download and save it — you will need to present it at the stadium entrance.
                  Each ticket has a unique barcode that will be scanned at the gate.
                </p>
              </td>
            </tr>
          </table>
        </td>
      </tr>

      <!-- Footer -->
      <tr>
        <td style=""background-color:#0b1530;padding:28px 40px;text-align:center;border-top:3px solid #d4af37;"">
          <p style=""margin:0 0 6px;font-size:14px;font-weight:700;color:#ffffff;"">
            EfraThib<span style=""color:#d4af37;"">_Ticketing</span>
          </p>
          <p style=""margin:0 0 12px;font-size:11px;color:#8fa3d0;"">
            Your central platform for UEFA Champions League tickets
          </p>
          <p style=""margin:0;font-size:11px;color:#4a5a7a;line-height:1.6;"">
            You received this email because you placed an order on EfraThib Ticketing.<br />
            &copy; {year} EfraThib_Ticketing. All rights reserved.
          </p>
        </td>
      </tr>

    </table>
  </td></tr>
</table>
</body>
</html>");

            return sb.ToString();
        }
    }
}
