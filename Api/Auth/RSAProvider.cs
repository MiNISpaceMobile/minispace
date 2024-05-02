using System.Security.Cryptography.X509Certificates;

namespace Api.Auth;

public class RSAProvider
{
    public X509Certificate2 Certificate { get; }

    public RSAProvider(IConfiguration config)
    {
        Certificate = X509Certificate2.CreateFromPem(config["RSA_CERTIFICATE_PEM"]!, config["RSA_PRIVATE_KEY_PEM"]!);
        // TODO: Actually add above variables to configuration, but not to the repo
    }
}