using Domain.Abstractions;

namespace Infrastructure.EmailSenders;

public class FakeEmailSender : IEmailSender
{
    public string From => "";

    public void SendEmail(string recipient, string subject, string content) { }
}
