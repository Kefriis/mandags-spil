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

        var smtpClient = new SmtpClient(_options.Host);

        smtpClient.Port = _options.Port;
        smtpClient.Credentials = new NetworkCredential(_options.SenderEmail, _options.EmailSenderPassword);
        smtpClient.EnableSsl = true;
        smtpClient.Timeout = 5000;
        smtpClient.UseDefaultCredentials = false;

        _smtpClient = smtpClient;
        _logger = logger;
    }
    
    public async Task SendResetPasswordEmailAsync(string toEmail, string token, string callBackUrl)
    {
        try
        {
            _logger.LogInformation("Sending email");

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_options.SenderEmail);
            mailMessage.Subject = "MandagSpil - Reset Password";

            var validCallback = callBackUrl + token;

            var emailBody = $"<h1>Password Reset Request</h1>" +
                $"<p>You requested a password reset. Please use the token below to reset your password:</p>" +
                $"<p>Token:</p>" +
                $"<p>{token}</p>" +
                $"<p>If you did not request a password reset, please ignore this email.</p>" +
                $"Link: <a href='{validCallback}'>{validCallback}</a>";

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
