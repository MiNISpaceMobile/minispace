namespace Api.DTO.Auth;

public record DTOLoginResponse(string loginToken, string loginSecret, string loginUrl);
