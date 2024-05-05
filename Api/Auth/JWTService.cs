using JWT;
using JWT.Algorithms;
using JWT.Builder;
using System.Security.Cryptography;

namespace Api.Auth;

public class JWTService
{
    public const string Issuer = "PW Minispace";
    public const string Audience = "PW Minispace";

    private IJwtAlgorithm algorithm;

    public JWTService(RSAProvider rsaProvider)
    {
        var rsa = RSA.Create(rsaProvider.Keys);
        algorithm = new RS256Algorithm(rsa, rsa);
    }

    public string Encode(Guid guid)
    {
        return JwtBuilder.Create()
            .WithAlgorithm(algorithm)
            .Issuer(Issuer)
            .Audience(Audience)
            .IssuedAt(DateTime.UtcNow)
            .ExpirationTime(DateTime.UtcNow.AddDays(1))
            .Subject(guid.ToString("N"))
            .Encode();
    }

    public Guid? Decode(string token)
    {
        try
        {
            var decoded = JwtBuilder.Create()
                .WithAlgorithm(algorithm)
                .MustVerifySignature()
                .WithValidationParameters(ValidationParameters.Default)
                .Decode<Dictionary<string, string>>(token);
            if (string.Equals(decoded["iss"], Issuer) &&
                string.Equals(decoded["aud"], Audience) &&
                Guid.TryParseExact(decoded["sub"], "N", out Guid guid))
                return guid;
        }
        catch { }
        return null;
    }
}
