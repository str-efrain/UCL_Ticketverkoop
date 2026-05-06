using Org.BouncyCastle.Utilities.Bzip2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util.Mail.Interfaces
{
    public interface IEmailSend
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendOrderConfirmationEmailAsync(string email, string subject, string message, byte[] pdfBytes);
    }
}
