using Domain.Abstractions;

namespace Infrastructure.EmailSenders;

public class FakeEmailSender : IEmailSender
{
    public void SendEmail(string recipient, string subject, string content) { }
}
