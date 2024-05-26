using Api.DTO.Users;

namespace Api.DTO.Notifications;

public record FriendRequestDto(
    Guid Guid,
    DateTime Timestamp,
    PublicUserDto User // target or source, depending whether we're listing received or sent friend requests
    ) : BaseNotificationDto(Guid, Timestamp);
