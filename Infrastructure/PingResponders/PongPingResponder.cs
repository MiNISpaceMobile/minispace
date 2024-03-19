using Domain.Abstractions;

namespace Infrastructure.PingResponders;

public class PongPingResponder : IPingResponder
{
    public string Response()
    {
        return "Pong";
    }
}
