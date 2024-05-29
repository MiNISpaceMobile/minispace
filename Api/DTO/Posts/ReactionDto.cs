using Api.DTO.Users;
using Domain.DataModel;

namespace Api.DTO.Posts;

public record ReactionDto(PublicUserDto Author, ReactionType Type);
