using Domain.BaseTypes;
using Domain.DataModel;

namespace Domain.Services;

public interface IEventService : IBaseService<IEventService>
{
    public List<Event> GetAll();
    public Event GetEvent(Guid guid);
    public Event CreateEvent(string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee);
    public void DeleteEvent(Guid guid);
    public void UpdateEvent(Event newEvent);
    public bool TryAddParticipant(Guid eventGuid);
    public bool TryRemoveParticipant(Guid eventGuid);
    public bool TryAddInterested(Guid eventGuid);
    public bool TryRemoveInterested(Guid eventGuid);
    public Feedback AddFeedback(Guid eventGuid, string content);
}
