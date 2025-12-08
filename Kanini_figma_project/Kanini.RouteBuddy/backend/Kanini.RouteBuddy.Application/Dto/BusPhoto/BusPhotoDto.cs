namespace Kanini.RouteBuddy.Application.Dto.BusPhoto;

public class BusPhotoDto
{
    public int BusPhotoId { get; set; }
    public string? ImagePath { get; set; }
    public string? Caption { get; set; }
    public int BusId { get; set; }
}