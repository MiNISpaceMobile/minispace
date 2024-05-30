using Api.DTO.Users;

namespace Api.DTO.Events;
public record FeedbackDto(
    Guid EventGuid,
    PublicUserDto User,
    int Rating);