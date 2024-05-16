﻿using System.Security.Cryptography;
using BLL.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Web_API.Helpers;

namespace BLL.Sevices;

public class EmailService(IOptions<SmtpConfig> smtpConfig)
{
    
    private readonly SmtpConfig _smtpConfig = smtpConfig.Value;

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
        email.From.Add(MailboxAddress.Parse(_smtpConfig.Username));
        email.To.Add(MailboxAddress.Parse(userModelEmail));
        email.Subject = "Email Confirmation";
        email.Body = new TextPart(TextFormat.Html)
        {
            Text = emailBody
        };

        using var smtp = new SmtpClient();
        smtp.Connect(_smtpConfig.Host, _smtpConfig.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_smtpConfig.Username, _smtpConfig.Password);

        smtp.Send(email);
        smtp.Disconnect(true);
    }
    
    public async Task CheckEmailAndToken(UserService userService, AuthService authService, string email, string token)
    {
        
        var user = await userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            throw new DataValidationException("Invalid email");
        }

        if (user.EmailResetToken != token)
        {
            throw new DataValidationException("Invalid token");
        }

        var alreadyVerified = await authService.ConfirmEmailAsync(user.UserId);
        if (alreadyVerified)
        {
            throw new DataValidationException("Email already verified");
        }
    }
}