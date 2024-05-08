using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IEventService : IBaseService<IEventService>
{
    public Event GetEvent(Guid guid);
    public Event CreateEvent(Guid studentGuid, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee);
    public void DeleteEvent(Guid guid);
    public void UpdateEvent(Event newEvent);
    public bool TryAddParticipant(Guid eventGuid, Guid studentGuid);
    public bool TryRemoveParticipant(Guid eventGuid, Guid studentGuid);
    public bool TryAddInterested(Guid eventGuid, Guid studentGuid);
    public bool TryRemoveInterested(Guid eventGuid, Guid studentGuid);
    public Feedback AddFeedback(Guid eventGuid, Guid authorGuid, string content);
}
