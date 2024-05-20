namespace Api.DTO.Notifications;

public class NotificationTimestampComparer : IComparer<NotificationDto>
{
    public static readonly NotificationTimestampComparer Instance = new();

    private NotificationTimestampComparer() { }

    public int Compare(NotificationDto? x, NotificationDto? y)
    {
        return -x!.Timestamp.CompareTo(y!.Timestamp);
    }
}
