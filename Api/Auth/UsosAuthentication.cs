using Api.DTO.Auth;
using Domain.Abstractions;
using Domain.DataModel;
using JWT.Algorithms;
using JWT.Builder;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;

namespace Api.Auth;

public class UsosAuthentication
{
    private IUnitOfWork uow;
    private RSAProvider rsa;

    private RestClient client;
    private string consumerKey;
    private string consumerSecret;

    public UsosAuthentication(IConfiguration config, IUnitOfWork uow, RSAProvider rsa)
    {
        this.uow = uow;
        this.rsa = rsa;
        client = new RestClient(config["USOS_BASE_URL"]!);
        consumerKey = config["USOS_CONSUMER_KEY"]!;
        // TODO: Actually add above variables to configuration, but not to the repo
        consumerSecret = config["USOS_CONSUMER_SECRET"]!;
    }

    // TODO: Proper error/exception handling

#pragma warning disable CS8618 // Unassigned non-nullables
    private class TokenResponse
    {
        [JsonPropertyName("oauth_token")]
        public string Token { get; set; }

        [JsonPropertyName("oauth_token_secret")]
        public string Secret { get; set; }
    }

    private class UserResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("birth_date")]
        public DateTime? DateOfBirth { get; set; }
    }
#pragma warning restore CS8618 // Unassigned non-nullables

    public DTOLoginResponse RequestLogin(DTOLoginRequest dtoRequest)
    {
        RestRequest request = new RestRequest("oauth/request_token")
        { Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, dtoRequest.callbackUrl) }
            .AddParameter("scopes", "email|personal");
        TokenResponse response = client.Post<TokenResponse>(request)!;
        return new DTOLoginResponse(response.Token, response.Secret, $"{client.Options.BaseUrl!.AbsoluteUri}oauth/authorize?oauth_token={response.Token}");
    }

    public DTOAccessResponse RequestAccess(DTOAccessRequest dtoRequest)
    {
        RestRequest accessRequest = new RestRequest("oauth/access_token")
        { Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, dtoRequest.loginToken, dtoRequest.loginSecret, dtoRequest.verifier) };
        TokenResponse accessResponse = client.Get<TokenResponse>(accessRequest)!;

        RestRequest userRequest = new RestRequest("users/user")
        { Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessResponse.Token, accessResponse.Secret) };
        UserResponse userResponse = client.Get<UserResponse>(userRequest)!;

        User? user = uow.Repository<User>().GetAll().Where(x => string.Equals(x.ExternalId, userResponse.Id)).SingleOrDefault();
        if (user is null) // first time user
        {
            user = new Student(userResponse.FirstName, userResponse.LastName, userResponse.Email) { DateOfBirth = userResponse.DateOfBirth };
            uow.Repository<Student>().Add((Student)user);
            uow.Commit();
        }

        string token = JwtBuilder.Create()
            .WithAlgorithm(new RS256Algorithm(rsa.Certificate))
            .Issuer("PW Minispace")
            .IssuedAt(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            .Subject(user.Guid.ToString())
            .AddClaim("usos_access_token", accessResponse.Token)
            .AddClaim("usos_access_secret", accessResponse.Secret)
            .Encode();
        return new DTOAccessResponse(token);
    }
}
