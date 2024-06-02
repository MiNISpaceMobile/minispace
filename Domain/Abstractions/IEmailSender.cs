namespace Domain.Abstractions;

public interface IEmailSender
{
    public const string From = "MiniSpace Team";

    public void SendEmail(string recipient, string subject, string content);
}
