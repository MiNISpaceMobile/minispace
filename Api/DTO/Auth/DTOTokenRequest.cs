using static Api.SwaggerConfig;

namespace Api.DTO.Auth;

public record DTOTokenRequest([DefaultApiValue("oob")] string CallbackUrl);
