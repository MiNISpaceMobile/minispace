namespace Api.DTO.Notifications;

public record FriendRequestDto(
    Guid Guid,
    Guid Target,
    Guid Source,
    DateTime Timestamp
    ) : BaseNotificationDto(Guid, Timestamp);
