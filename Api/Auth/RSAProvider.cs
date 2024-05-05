using System.Security.Cryptography;

namespace Api.Auth;

public class RSAProvider
{
    public RSAParameters Keys { get; }

    public RSAProvider(IConfiguration config)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(config["RSA_KEYS_PEM"]!);
        Keys = rsa.ExportParameters(true);
    }
}