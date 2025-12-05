using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Kanini.RouteBuddy.Application.Dto.BusPhoto;

public class CreateBusPhotoDto
{
    [Required(ErrorMessage = "Bus ID is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Bus ID must be a positive number")]
    public int BusId { get; set; }
    
    [Required(ErrorMessage = "Photo is required")]
    public IFormFile Photo { get; set; } = null!;
    
    [MaxLength(100, ErrorMessage = "Caption cannot exceed 100 characters")]
    public string? Caption { get; set; }
}