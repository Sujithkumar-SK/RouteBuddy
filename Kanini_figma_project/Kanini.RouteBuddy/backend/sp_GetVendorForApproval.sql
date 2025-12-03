-- Stored Procedure: Get Vendor For Approval with Documents
CREATE OR ALTER PROCEDURE sp_GetVendorForApproval
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get vendor details
    SELECT 
        v.VendorId,
        v.UserId,
        v.AgencyName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.OfficeAddress,
        v.FleetSize,
        v.TaxRegistrationNumber,
        v.Status,
        v.IsActive,
        v.CreatedOn,
        u.Email,
        u.Phone,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Rejected'
        END as StatusText
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId;
    
    -- Get vendor documents
    SELECT 
        vd.DocumentId,
        vd.VendorId,
        vd.DocumentFile,
        vd.DocumentPath,
        vd.IssueDate,
        vd.ExpiryDate,
        vd.UploadedAt,
        vd.IsVerified,
        vd.VerifiedAt,
        vd.VerifiedBy,
        vd.RejectedReason,
        vd.Status,
        CASE vd.DocumentFile
            WHEN 0 THEN 'BusinessLicense'
            WHEN 1 THEN 'TaxRegistration'
            WHEN 2 THEN 'IdentityProof'
            WHEN 3 THEN 'AddressProof'
        END as DocumentType
    FROM VendorDocuments vd
    WHERE vd.VendorId = @VendorId
    ORDER BY vd.UploadedAt DESC;
END