using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class SeatLayoutTemplateResponseDto
{
    public int SeatLayoutTemplateId { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public int TotalSeats { get; set; }
    public BusType BusType { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public List<SeatLayoutDetailResponseDto> SeatDetails { get; set; } = new();
}