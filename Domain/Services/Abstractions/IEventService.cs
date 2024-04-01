using Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services;

public interface IEventService
{
    public Event? GetEvent(Guid guid);
    public void CreateEvent(Event newEvent);
    //public void UpdateEvent();
}
