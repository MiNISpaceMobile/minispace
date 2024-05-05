using Api.DTO.Auth;
using Domain.Abstractions;
using Domain.DataModel;
using RestSharp;
using RestSharp.Authenticators;
using System.Text.Json.Serialization;
using System.Web;

namespace Api.Auth;

public class UsosAuthentication
{
    private IUnitOfWork uow;
    private JwtService jwtService;

    private RestClient client;
    private string consumerKey;
    private string consumerSecret;

    public UsosAuthentication(IConfiguration config, IUnitOfWork uow, JwtService jwtService)
    {
        this.uow = uow;
        this.jwtService = jwtService;

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
        public string? Email { get; set; }

        [JsonPropertyName("birth_date")]
        public DateTime? DateOfBirth { get; set; }

        [JsonConstructor]
        public UserResponse(string id, string firstName, string lastName, string email, DateTime? dateOfBirth)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DateOfBirth = dateOfBirth;
        }
    }

    public DTOLoginResponse RequestLogin(DTOLoginRequest dtoRequest)
    {
        if (!Uri.IsWellFormedUriString(dtoRequest.callbackUrl, UriKind.RelativeOrAbsolute) && !string.Equals(dtoRequest.callbackUrl, "oob"))
            throw new ArgumentException("callbackUrl is neither a valid Url nor 'oob'");

        RestRequest request = new RestRequest("oauth/request_token")
        { Authenticator = OAuth1Authenticator.ForRequestToken(consumerKey, consumerSecret, dtoRequest.callbackUrl) }
            .AddParameter("scopes", "email|personal");
        TokenResponse response = new TokenResponse(client.Post(request));

        return new DTOLoginResponse(response.Token, response.Secret, $"{client.Options.BaseUrl!.AbsoluteUri}oauth/authorize?oauth_token={response.Token}");
    }

    public DTOAccessResponse RequestAccess(DTOAccessRequest dtoRequest)
    {
        RestRequest accessRequest = new RestRequest("oauth/access_token")
        { Authenticator = OAuth1Authenticator.ForAccessToken(consumerKey, consumerSecret, dtoRequest.loginToken, dtoRequest.loginSecret, dtoRequest.verifier) };
        TokenResponse accessResponse = new TokenResponse(client.Get(accessRequest)!);

        RestRequest userRequest = new RestRequest("users/user")
        { Authenticator = OAuth1Authenticator.ForProtectedResource(consumerKey, consumerSecret, accessResponse.Token, accessResponse.Secret) }
            .AddParameter("fields", "id|first_name|last_name|email|birth_date");
        UserResponse userResponse = client.Get<UserResponse>(userRequest)!;
        if (string.IsNullOrEmpty(userResponse.Id))
            throw new Exception("Unexpected expection - USOS API request succeeded, but returned ID is invalid, possible deserialization error");

        User? user = uow.Repository<User>().GetAll().Where(x => string.Equals(x.ExternalId, userResponse.Id)).SingleOrDefault();
        if (user is null) // first time user
        {
            user = new Student(userResponse.FirstName, userResponse.LastName, userResponse.Email ?? "", userResponse.Id) { DateOfBirth = userResponse.DateOfBirth };
            uow.Repository<Student>().Add((Student)user);
            uow.Commit();
        }

        return new DTOAccessResponse(jwtService.Encode(user.Guid));
    }
}
