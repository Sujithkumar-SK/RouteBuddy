using System;
using System.Collections.Generic;
using SmartRouteEngine.Models;
using SmartRouteEngine.Services;

class Program
{
  static void Main()
  {
    var buses = new List<BusRoute>
        {
            
            new BusRoute { BusId=1, Source="Chennai", Destination="Madurai", SeatsAvailable=0,
                DepartureTime = new DateTime(2025, 8, 30, 06, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 12, 00, 00) }, 

            new BusRoute { BusId=2, Source="Chennai", Destination="Salem", SeatsAvailable=15,
                DepartureTime = new DateTime(2025, 8, 30, 07, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 11, 00, 00) },

          
            new BusRoute { BusId=3, Source="Salem", Destination="Coimbatore", SeatsAvailable=20,
                DepartureTime = new DateTime(2025, 8, 30, 12, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 15, 00, 00) },

            new BusRoute { BusId=4, Source="Coimbatore", Destination="Madurai", SeatsAvailable=10,
                DepartureTime = new DateTime(2025, 8, 30, 16, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 20, 00, 00) },

            
            new BusRoute { BusId=5, Source="Salem", Destination="Madurai", SeatsAvailable=12,
                DepartureTime = new DateTime(2025, 8, 30, 10, 00, 00), 
                ArrivalTime   = new DateTime(2025, 8, 30, 14, 00, 00) },

            new BusRoute { BusId=6, Source="Chennai", Destination="Trichy", SeatsAvailable=18,
                DepartureTime = new DateTime(2025, 8, 30, 08, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 12, 00, 00) },

            new BusRoute { BusId=7, Source="Trichy", Destination="Madurai", SeatsAvailable=18,
                DepartureTime = new DateTime(2025, 8, 30, 13, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 16, 00, 00) },

            
            new BusRoute { BusId=8, Source="Chennai", Destination="Vellore", SeatsAvailable=10,
                DepartureTime = new DateTime(2025, 8, 30, 09, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 11, 00, 00) },

            new BusRoute { BusId=9, Source="Vellore", Destination="Erode", SeatsAvailable=5,
                DepartureTime = new DateTime(2025, 8, 30, 12, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 15, 00, 00) }, 

            
            new BusRoute { BusId=10, Source="Madurai", Destination="Chennai", SeatsAvailable=20,
                DepartureTime = new DateTime(2025, 8, 30, 22, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 31, 04, 00, 00) },

            new BusRoute { BusId=11, Source="Thanjavur", Destination="Madurai", SeatsAvailable=8,
                DepartureTime = new DateTime(2025, 8, 30, 14, 00, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 17, 00, 00) },

            new BusRoute { BusId=12, Source="Chennai", Destination="Thanjavur", SeatsAvailable=15,
                DepartureTime = new DateTime(2025, 8, 30, 08, 30, 00),
                ArrivalTime   = new DateTime(2025, 8, 30, 13, 00, 00) }
        };

    var engine = new SmartEngine(buses);

    // === TEST CASES ===
    RunTest(engine, "Chennai", "Madurai", new DateTime(2025, 8, 30)); 
    RunTest(engine, "Chennai", "Salem", new DateTime(2025, 8, 30));  
    RunTest(engine, "Chennai", "Coimbatore", new DateTime(2025, 8, 30)); 
    RunTest(engine, "Chennai", "Vellore", new DateTime(2025, 8, 30));
    RunTest(engine, "Thanjavur", "Madurai", new DateTime(2025, 8, 30)); 
  }

  static void RunTest(SmartEngine engine, string start, string end, DateTime date)
  {
    Console.WriteLine($"\n=== Searching route from {start} to {end} on {date:dd-MMM-yyyy} ===");

    var route = engine.FindRoute(start, end, date);

    if (route != null)
    {
      Console.WriteLine("Smart Route Found:");
      foreach (var bus in route)
      {
        Console.WriteLine($"Bus {bus.BusId}: {bus.Source} -> {bus.Destination} " +
                          $"[{bus.DepartureTime:HH:mm} - {bus.ArrivalTime:HH:mm}] | Seats: {bus.SeatsAvailable}");
      }
    }
    else
    {
      Console.WriteLine("❌ No valid route found.");
    }
  }
}
