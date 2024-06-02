namespace Domain.Abstractions;

public interface IEmailSender
{
    public string From { get; }

    public void SendEmail(string recipient, string subject, string content);
}
