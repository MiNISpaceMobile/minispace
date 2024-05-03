using System.Security.Cryptography;

namespace Api.Auth;

public class RSAProvider
{
    public RSA Keys { get; }

    public RSAProvider(IConfiguration config)
    {
        Keys = RSA.Create();
        Keys.ImportFromPem(config["RSA_KEYS_PEM"]!);
    }
}