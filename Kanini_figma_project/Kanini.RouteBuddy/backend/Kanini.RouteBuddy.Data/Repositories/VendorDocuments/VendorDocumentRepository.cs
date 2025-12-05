using Kanini.RouteBuddy.Data.DatabaseContext;
using VendorDocumentEntity = Kanini.RouteBuddy.Domain.Entities.VendorDocument;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Common;

namespace Kanini.RouteBuddy.Data.Repositories.VendorDocuments;

public class VendorDocumentRepository : IVendorDocumentRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly ILogger<VendorDocumentRepository> _logger;

    public VendorDocumentRepository(RouteBuddyDatabaseContext context, ILogger<VendorDocumentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task AddVendorDocumentAsync(VendorDocumentEntity document)
    {
        try
        {
            _context.VendorDocuments.Add(document);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Vendor document added for VendorId: {VendorId}", document.VendorId);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while adding vendor document for VendorId: {VendorId}", document.VendorId);
            throw new Exception(MagicStrings.ErrorMessages.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while adding vendor document for VendorId: {VendorId}", document.VendorId);
            throw new Exception(MagicStrings.ErrorMessages.InternalServerError);
        }
    }

    public async Task<List<VendorDocumentEntity>> GetVendorDocumentsByVendorIdAsync(int vendorId)
    {
        try
        {
            return await _context.VendorDocuments
                .Where(vd => vd.VendorId == vendorId)
                .OrderByDescending(vd => vd.CreatedOn)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving vendor documents for VendorId: {VendorId}", vendorId);
            throw new Exception(MagicStrings.ErrorMessages.DatabaseError);
        }
    }
}