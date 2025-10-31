using System.ComponentModel.DataAnnotations;

namespace Kanini.RouteBuddy.Domain.Entities;

public abstract class BaseEntity
{
    [MaxLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
}
