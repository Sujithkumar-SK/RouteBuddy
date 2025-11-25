using Kanini.RouteBuddy.Domain.Enums;

namespace Kanini.RouteBuddy.Application.Dto.Vendor;

public class VendorDocumentDTO
{
    public int DocumentId { get; set; }
    public DocumentCategory DocumentFile { get; set; }
    public string DocumentPath { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime UploadedAt { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public DocumentStatus Status { get; set; }
}