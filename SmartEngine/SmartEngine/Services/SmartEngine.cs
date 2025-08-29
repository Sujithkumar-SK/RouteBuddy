using System;
using System.Xml;
using SmartRouteEngine.Models;

namespace SmartRouteEngine.Services;

public class SmartEngine
{
  private readonly List<BusRoute> _routes;
  private readonly TimeSpan transerBuffer = TimeSpan.FromMinutes(15);

  public SmartEngine(List<BusRoute> routes)
  {
    _routes = routes;
  }

  public List<BusRoute> FindRoute(string start, string end, DateTime journeyDate)
  {
    var direct = _routes.FirstOrDefault(r =>
        r.Source == start
        && r.Destination == end
        && r.SeatsAvailable > 0
        && r.DepartureTime.Date == journeyDate.Date
    );
    if (direct != null)
    {
      return new List<BusRoute> { direct };
    }
    var queue = new Queue<List<BusRoute>>();
    foreach (var i in _routes.Where(i =>
      i.Source == start && i.SeatsAvailable > 0 && i.DepartureTime.Date == journeyDate.Date))
    {
      queue.Enqueue(new List<BusRoute> { i });
    }
    List<BusRoute> bestPath = null;
    while (queue.Count > 0)
    {
      var path = queue.Dequeue();
      var lastBus = path.Last();
      if (lastBus.Destination == end)
      {
        if (bestPath == null || lastBus.ArrivalTime < bestPath.Last().ArrivalTime || (lastBus.ArrivalTime == bestPath.Last().ArrivalTime && path.Count < bestPath.Count))
        {
          bestPath = path;
        }
        continue;
      }
      foreach (var next in _routes.Where(r =>
        r.Source == lastBus.Destination &&
        r.SeatsAvailable > 0 &&
        r.DepartureTime >= lastBus.ArrivalTime + transerBuffer))
      {
        var newPath = new List<BusRoute>(path) { next };
        queue.Enqueue(newPath);
      }
    }
    return bestPath;
  }

}
