using System;

namespace MandagsSpil.Api.Services;

public class MailOptions
{
  public const string Mail = "MailOptions";

    public string EmailSenderPassword { get; set; } = String.Empty;
    public string SenderEmail { get; set; } = String.Empty;
}
