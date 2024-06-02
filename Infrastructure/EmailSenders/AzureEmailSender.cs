using Azure.Communication.Email;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EmailSenders;

public class AzureEmailSender : IEmailSender
{
    private EmailClient email;

    public AzureEmailSender(IConfiguration config)
    {
        email = new EmailClient(config["AZURE_COMMUNICATION_CONNECTIONSTRING"]!);
    }

    public void SendEmail(string recipient, string subject, string content)
    {
        email.Send(Azure.WaitUntil.Started, IEmailSender.From, recipient, subject, content);
    }
}
