using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class SeatLayoutDetailResponseDto
{
    public int SeatLayoutDetailId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatType SeatType { get; set; }
    public SeatPosition SeatPosition { get; set; }
    public int RowNumber { get; set; }
    public int ColumnNumber { get; set; }
    public PriceTier PriceTier { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}