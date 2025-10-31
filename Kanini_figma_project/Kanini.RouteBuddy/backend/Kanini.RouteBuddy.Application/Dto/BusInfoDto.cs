using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class BusInfoDto
{
    public string BusName { get; set; } = string.Empty;
    public BusType BusType { get; set; }
    public BusAmenities BusAmenities { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public int BookedSeats { get; set; }
    public decimal BasePrice { get; set; }
}
