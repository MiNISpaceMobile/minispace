using Domain.Abstractions;

namespace Infrastructure.PingResponders;

public class PongPingResponder : IPingResponder
{
    public string Response()
    {
        return "Pong";
    }
    public string Response(Guid guid)
    {
        return $"Pong to {guid}";
    }
}
