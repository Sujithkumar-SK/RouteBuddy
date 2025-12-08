using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto.BusPhoto;

public class UpdateBusPhotoDto
{
    [MaxLength(100)]
    public string? Caption { get; set; }
}