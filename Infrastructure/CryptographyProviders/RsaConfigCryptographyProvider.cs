using Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace Infrastructure.CryptographyProviders;

public class RsaConfigCryptographyProvider : ICryptographyProvider<RSAParameters>
{
    public RSAParameters Keys { get; }

    public RsaConfigCryptographyProvider(IConfiguration config)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(config["RSA_KEYS_PEM"]!);
        Keys = rsa.ExportParameters(true);
    }
}