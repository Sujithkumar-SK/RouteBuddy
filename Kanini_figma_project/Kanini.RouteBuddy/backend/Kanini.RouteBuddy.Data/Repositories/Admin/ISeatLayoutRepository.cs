using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Domain.Entities;

namespace Kanini.RouteBuddy.Data.Repositories.Admin;

public interface ISeatLayoutRepository
{
    Task<Result<List<SeatLayoutTemplate>>> GetAllTemplatesAsync();
    Task<Result<SeatLayoutTemplate>> GetTemplateByIdAsync(int templateId);
    Task<Result<SeatLayoutTemplate>> GetTemplateWithDetailsAsync(int templateId);

    Task<Result<SeatLayoutTemplate>> CreateTemplateAsync(
        SeatLayoutTemplate template,
        List<SeatLayoutDetail> details
    );
    Task<Result<SeatLayoutTemplate>> UpdateTemplateAsync(
        SeatLayoutTemplate template,
        List<SeatLayoutDetail> details
    );
    Task<Result<bool>> DeactivateTemplateAsync(int templateId);
    Task<Result<bool>> CheckTemplateNameExistsAsync(
        string templateName,
        int? excludeTemplateId = null
    );
    Task<Result<List<SeatLayoutTemplate>>> GetTemplatesByBusTypeAsync(int busType);
}
