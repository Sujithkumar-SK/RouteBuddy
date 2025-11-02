using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto;

public class SeatLayoutDetailRequestDto
{
    [Required(ErrorMessage = "Seat number is required")]
    [StringLength(10, MinimumLength = 2, ErrorMessage = "Seat number must be between 2 and 10 characters")]
    [RegularExpression(@"^[A-Z][0-9]+[A-Z]*$", ErrorMessage = "Seat number format should be like A1, B2U, C3L")]
    public string SeatNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seat type is required")]
    public SeatType SeatType { get; set; }

    [Required(ErrorMessage = "Seat position is required")]
    public SeatPosition SeatPosition { get; set; }

    [Required(ErrorMessage = "Row number is required")]
    [Range(1, 50, ErrorMessage = "Row number must be between 1 and 50")]
    public int RowNumber { get; set; }

    [Required(ErrorMessage = "Column number is required")]
    [Range(1, 10, ErrorMessage = "Column number must be between 1 and 10")]
    public int ColumnNumber { get; set; }

    [Required(ErrorMessage = "Price tier is required")]
    public PriceTier PriceTier { get; set; } = PriceTier.Base;
}