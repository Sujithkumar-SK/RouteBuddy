using VendorDocumentEntity = Kanini.RouteBuddy.Domain.Entities.VendorDocument;

namespace Kanini.RouteBuddy.Data.Repositories.VendorDocuments;

public interface IVendorDocumentRepository
{
    Task AddVendorDocumentAsync(VendorDocumentEntity document);
    Task<List<VendorDocumentEntity>> GetVendorDocumentsByVendorIdAsync(int vendorId);
}