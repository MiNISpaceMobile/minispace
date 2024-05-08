namespace Api.DTO.Auth;

public record DTOTokenResponse(string Token, string Secret, string Url);
