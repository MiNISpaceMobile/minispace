namespace Api.DTO.Notifications;

public record NotificationDto(Guid Guid, Guid Source, string Type, bool Seen, DateTime Timestamp);
