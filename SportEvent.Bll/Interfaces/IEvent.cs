using SportEvent.Bll.Model;

namespace SportEvent.Bll.Interfaces
{
    public interface IEvent
    {
        EventModel AddEvent(EventModel model);
        EventModel RemoveEvent(EventModel model);
    }
}
