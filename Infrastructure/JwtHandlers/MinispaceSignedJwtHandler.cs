using Domain.Abstractions;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using System.Security.Cryptography;

namespace Infrastructure.JwtHandlers;

public class MinispaceSignedJwtHandler : IJwtHandler
{
    public const string Issuer = "PW Minispace";
    public const string Audience = "PW Minispace";

    private IJwtAlgorithm algorithm;

    public MinispaceSignedJwtHandler(ICryptographyProvider<RSAParameters> rsaProvider)
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
                .WithValidationParameters(ValidationParameters.Default)
                .MustVerifySignature()
                .Decode<Dictionary<string, object>>(token);
            if (Equals(decoded["iss"], Issuer) &&
                Equals(decoded["aud"], Audience) &&
                Guid.TryParseExact(decoded["sub"].ToString(), "N", out Guid guid))
                return guid;
        }
        catch { }
        return null;
    }

    public (Guid guid, bool expired)? DecodeEvenIfExpired(string token)
    {
        try
        {
            ValidationParameters vp = ValidationParameters.Default
                .With(options => options.ValidateExpirationTime = false);
            var decoded = JwtBuilder.Create()
                .WithAlgorithm(algorithm)
                .WithValidationParameters(vp)
                .MustVerifySignature()
                .Decode<Dictionary<string, object>>(token);
            double expUnixTime = Convert.ToDouble(decoded["exp"]);
            double nowUnixTime = UnixEpoch.GetSecondsSince(DateTimeOffset.UtcNow);
            bool expired = expUnixTime <= nowUnixTime;
            if (Equals(decoded["iss"], Issuer) &&
                Equals(decoded["aud"], Audience) &&
                Guid.TryParseExact(decoded["sub"].ToString(), "N", out Guid guid))
                return (guid, expired);
        }
        catch { }
        return null;
    }
}
