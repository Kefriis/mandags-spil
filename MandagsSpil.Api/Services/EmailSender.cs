using System;
using System.Net;
using System.Net.Mail;
using MandagsSpil.Api.Interfaces;
using Microsoft.Extensions.Options;

namespace MandagsSpil.Api.Services;

public class EmailSender : IEmailSender
{
    private readonly SmtpClient _smtpClient;
    private readonly ILogger<EmailSender> _logger;
    private readonly MailOptions _options;

    public EmailSender(ILogger<EmailSender> logger, IOptions<MailOptions> options)
    {
        _options = options.Value;
        // var emailSenderPassword = "ffdd unvg qwtv ailr";

        var smtpClient = new SmtpClient("smtp.gmail.com");

        smtpClient.Port = 587;
        smtpClient.Credentials = new NetworkCredential(_options.SenderEmail, _options.EmailSenderPassword);
        smtpClient.EnableSsl = true;
        smtpClient.Timeout = 5000;
        smtpClient.UseDefaultCredentials = false;

        _smtpClient = smtpClient;
        _logger = logger;
    }
    
    public async Task SendResetPasswordEmailAsync(string toEmail, string token)
    {
        try
        {
            _logger.LogInformation("Sending email");

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_options.SenderEmail);
            mailMessage.Subject = "MandagSpil - Reset Password";

            var emailBody = $"<h1>Password Reset Request</h1>" +
                $"<p>You requested a password reset. Please use the token below to reset your password:</p>" +
                $"<p>Token:</p>" +
                $"<p>{token}</p>";

            mailMessage.Body = emailBody;
            mailMessage.IsBodyHtml = true;
            mailMessage.To.Add(toEmail);

            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
        }

    }
}
