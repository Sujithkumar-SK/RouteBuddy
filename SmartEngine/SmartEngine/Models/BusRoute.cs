using System;

namespace SmartRouteEngine.Models
{
    public class BusRoute
    {
        public int BusId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int SeatsAvailable { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}
