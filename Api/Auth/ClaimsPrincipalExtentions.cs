using System.Security.Claims;

namespace Api.Auth;

public static class Extentions
{
    public static Guid GetGuid(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirstValue(JwtAuthScheme.GuidClaim)!);
}
