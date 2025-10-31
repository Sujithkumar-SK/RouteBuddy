using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class SeatDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public SeatType SeatType { get; set; }
    public SeatPosition SeatPosition { get; set; }
    public int RowNumber { get; set; }
    public int ColumnNumber { get; set; }
    public PriceTier PriceTier { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBooked { get; set; }
    public decimal Price { get; set; }
}
