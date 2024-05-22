using System.Security.Claims;
using Api.Auth;

namespace Api;

public static class Extensions
{
    public static Guid GetGuid(this ClaimsPrincipal user)
        => Guid.Parse(user.FindFirstValue(JwtAuthScheme.GuidClaim)!);
}
