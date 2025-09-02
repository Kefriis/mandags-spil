using System;

namespace MandagsSpil.Api.Interfaces;

public interface IEmailSender
{
    Task SendResetPasswordEmailAsync(string toEmail, string token, string callBackUrl);
}
