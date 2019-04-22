using System;
using System.Collections.Generic;
using System.Text;

namespace SportEvent.Bll.Model
{
    public class EventModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public decimal? EventCost { get; set; }
    }
}
