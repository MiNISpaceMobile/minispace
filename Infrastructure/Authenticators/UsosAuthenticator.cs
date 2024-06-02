using Domain.Abstractions;
using Domain.DataModel;
using RestSharp.Authenticators;
using RestSharp;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Authenticators;

public class UsosAuthenticator : Domain.Abstractions.IAuthenticator
{
    private IUnitOfWork uow;

    private RestClient client;
    private string consumerKey;
    private string consumerSecret;

    public UsosAuthenticator(IConfiguration config, IUnitOfWork uow)
    {
        this.uow = uow;

        client = new RestClient(config["USOS_BASE_URL"]!, options => options.ThrowOnAnyError = true);
        consumerKey = config["USOS_CONSUMER_KEY"]!;
        consumerSecret = config["USOS_CONSUMER_SECRET"]!;
    }

    private class TokenResponse
    {
        public string Token { get; set; }
        public string Secret { get; set; }

        public TokenResponse(string token, string secret)
        {
            Token = token;
            Secret = secret;
        }
        public TokenResponse(RestResponse response)
        {
            var query = HttpUtility.ParseQueryString(response.Content!);
            Token = query.Get("oauth_token")!;
            Secret = query.Get("oauth_token_secret")!;
        }
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
        public DateTime DateOfBirth { get; set; }

        [JsonConstructor]
        public UserResponse(string id, string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateOfBirth = dateOfBirth;
        }
    }

    public (string token, string secret, string url) RequestAuthenticationToken(string callbackUrl)
    {
        if (!Uri.IsWellFormedUriString(callbackUrl, UriKind.RelativeOrAbsolute) && !string.Equals(callbackUrl, "oob"))
            throw new ArgumentException("callbackUrl is neither a valid Url nor 'oob'");

        RestRequest request = new RestRequest("oauth/request_token")
        { Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, callbackUrl) }
            .AddParameter("scopes", "email|personal");
        RestResponse response = client.Post(request);

        var query = HttpUtility.ParseQueryString(response.Content!);
        string token = query.Get("oauth_token")!;
        string secret = query.Get("oauth_token_secret")!;
        string url = $"{client.Options.BaseUrl!.AbsoluteUri}oauth/authorize?oauth_token={token}";

        return (token, secret, url);
    }

    public Guid Authenticate(string token, string secret, string verifier)
    {
        RestRequest accessRequest = new RestRequest("oauth/access_token")
        { Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, token, secret, verifier) };
        RestResponse accessResponse = client.Get(accessRequest)!;

        var query = HttpUtility.ParseQueryString(accessResponse.Content!);
        token = query.Get("oauth_token")!;
        secret = query.Get("oauth_token_secret")!;

        RestRequest userRequest = new RestRequest("users/user")
        { Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, token, secret) }
            .AddParameter("fields", "id|first_name|last_name|email|birth_date");
        UserResponse userResponse = client.Get<UserResponse>(userRequest)!;
        if (string.IsNullOrEmpty(userResponse.Id))
            throw new Exception("Unexpected expection - USOS API request succeeded, but returned ID is invalid");

        User? user = uow.Repository<User>().GetAll().Where(x => string.Equals(x.ExternalId, userResponse.Id)).SingleOrDefault();
        if (user is null) // first time user
        {
            user = new User(userResponse.FirstName, userResponse.LastName, userResponse.Email, userResponse.DateOfBirth, userResponse.Id);
            uow.Repository<User>().Add(user);
            uow.Commit();
        }
        else // update existing user
        {
            user.FirstName = userResponse.FirstName;
            user.LastName = userResponse.LastName;
            user.Email = userResponse.Email;
            user.DateOfBirth = userResponse.DateOfBirth;
            uow.Commit();
        }

        return user.Guid;
    }
}
