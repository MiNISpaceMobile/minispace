using Azure.Communication.Email;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EmailSenders;

public class AzureEmailSender : IEmailSender
{
    private EmailClient email;

    public string From => "MiniSpaceTeam@dea7d9fb-220c-4c7c-ad69-b466a0daa2ea.azurecomm.net";

    public AzureEmailSender(IConfiguration config)
    {
        email = new EmailClient(config["AZURE_COMMUNICATION_CONNECTIONSTRING"]!);
    }

    public void SendEmail(string recipient, string subject, string content)
    {
        email.Send(Azure.WaitUntil.Completed, From, recipient, subject, content);
    }
}
