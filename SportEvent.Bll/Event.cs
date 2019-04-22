using SportEvent.Bll.Interfaces;
using SportEvent.Bll.Model;

namespace SportEvent.Bll
{
    public class Event : IEvent
    {
        public EventModel AddEvent(EventModel model)
        {
            return model;
        }

        public EventModel RemoveEvent(EventModel model)
        {
            return model;
        }
        
    }
}
