using System.ComponentModel.DataAnnotations;
using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Bus;

public class UpdateBusDto
{
    [Required, MaxLength(100)]
    public string BusName { get; set; } = null!;

    [Required]
    public BusType BusType { get; set; }

    [Required, Range(1, 100)]
    public int TotalSeats { get; set; }

    [MaxLength(500)]
    public string? RegistrationPath { get; set; }

    public BusAmenities Amenities { get; set; }

    [MaxLength(100)]
    public string? DriverName { get; set; }

    [Phone]
    public string? DriverContact { get; set; }

    public bool IsActive { get; set; }
}