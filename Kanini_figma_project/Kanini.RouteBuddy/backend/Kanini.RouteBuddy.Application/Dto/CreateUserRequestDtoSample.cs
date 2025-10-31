using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Application.Dto
{
    public class CreateUserRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }
    }
}
