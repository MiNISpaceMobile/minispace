namespace Api.DTO.Auth;

public record DTOJwtRequest(string Token, string Secret, string Verifier);
