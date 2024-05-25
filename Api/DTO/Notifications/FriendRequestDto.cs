using Api.DTO.Users;

namespace Api.DTO.Notifications;

public record FriendRequestDto(
    Guid Guid,
    Guid Target,
    DateTime Timestamp,
    PublicUserDto Source
    ) : BaseNotificationDto(Guid, Timestamp);
