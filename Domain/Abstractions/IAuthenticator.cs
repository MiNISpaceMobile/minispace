namespace Domain.Abstractions;

public interface IAuthenticator
{
    public (string token, string secret, string url) RequestAuthenticationToken(string callbackUrl);
    public Guid Authenticate(string token, string secret, string verifier);
}
