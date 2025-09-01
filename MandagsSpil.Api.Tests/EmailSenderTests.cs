using System;
using MandagsSpil.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace MandagsSpil.Api.Tests;

public class EmailSenderTests
{
    [Test]
    public async Task Test1()
    {
        var mailOptions = Substitute.For<IOptions<MailOptions>>();

        mailOptions.Value.Returns(new MailOptions());

        var ti = new EmailSender(new NullLogger<EmailSender>(), mailOptions);

        await ti.SendResetPasswordEmailAsync("friis1212@hotmail.com", "yay, this should be a random token, wheeee");

        Assert.Pass();
    }
}
