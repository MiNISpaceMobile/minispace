using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public interface IEventService
{
    public Event GetEvent(Guid guid);
    public Event CreateEvent(Guid studentGuid, string title, string description, EventCategory category, DateTime publicationDate,
                 DateTime startDate, DateTime endDate, string location, int? capacity, decimal? fee);
    public void UpdateEvent(Event newEvent);
    public bool TryAddParticipant(Guid eventGuid, Guid studentGuid);
    public bool TryAddInterested(Guid eventGuid, Guid studentGuid);
}
