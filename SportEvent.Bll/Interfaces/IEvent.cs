using SportEvent.Bll.Model;

namespace SportEvent.Bll.Interfaces
{
    public interface IEvent
    {
        string AddEvent(EventModel model);
        string RemoveEvent(EventModel model);
    }
}
