namespace Domain.Abstractions;

public interface IJwtHandler
{
    public static string StripAuthSchemeName(string token, string authSchemeName)
        => token.StartsWith(authSchemeName) ? token[authSchemeName.Length..].Trim() : token;

    public string Encode(Guid guid, TimeSpan? expireAfter = null);
    public Guid? Decode(string token);
    public (Guid guid, bool expired)? DecodeEvenIfExpired(string token);
}
