namespace Api.DTO.Notifications;

public class BaseNotificationTimestampComparer : IComparer<BaseNotificationDto>
{
    public static readonly BaseNotificationTimestampComparer Instance = new();

    private BaseNotificationTimestampComparer() { }

    public int Compare(BaseNotificationDto? x, BaseNotificationDto? y)
    {
        return -x!.Timestamp.CompareTo(y!.Timestamp);
    }
}
