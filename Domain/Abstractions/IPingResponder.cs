namespace Domain.Abstractions;

public interface IPingResponder
{
    public string Response();
    public string Response(Guid guid);
}
