using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Domain.Entities;

[Table("RefreshTokens")]
[Index(nameof(Token), IsUnique = true, Name = "IX_RefreshTokens_Token")]
[Comment("Stores refresh tokens for JWT authentication")]
public class RefreshToken : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RefreshTokenId { get; set; }

    [Required]
    [MaxLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string Token { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [Column(TypeName = "DATETIME2")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "BIT")]
    public bool IsRevoked { get; set; } = false;

    [Column(TypeName = "DATETIME2")]
    public DateTime? RevokedAt { get; set; }

    [MaxLength(50)]
    [Column(TypeName = "NVARCHAR(50)")]
    public string? ReplacedByToken { get; set; }

    [NotMapped]
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
}
