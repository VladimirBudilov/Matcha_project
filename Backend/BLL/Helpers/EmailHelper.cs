using System.Security.Cryptography;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace BLL.Helpers;

public class EmailHelper
{
    public string GenerateEmailConfirmationToken()
    {
        using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
        {
            var randomNumber = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }


    public void SendEmail(string userModelEmail, string emailBody)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse("annette.lowe20@ethereal.email"));
        email.To.Add(MailboxAddress.Parse("annette.lowe20@ethereal.email"));
        email.Subject = "Email Confirmation";
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.Connect("smtp.ethereal.email", 587, SecureSocketOptions.StartTls);
        smtp.Authenticate("annette.lowe20@ethereal.email", "uJ5qF5D81yYtUe7Nsf");

        smtp.Send(email);
        smtp.Disconnect(true);
    }
}