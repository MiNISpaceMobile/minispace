namespace Api.DTO.Auth;

public record DTOAccessRequest(string loginToken, string loginSecret, string verifier);
