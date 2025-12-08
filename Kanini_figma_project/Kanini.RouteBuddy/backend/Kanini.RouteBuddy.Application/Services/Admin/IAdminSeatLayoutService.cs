using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common.Utility;

namespace Kanini.RouteBuddy.Application.Services.Admin;

public interface IAdminSeatLayoutService
{
    Task<Result<List<SeatLayoutTemplateListDto>>> GetAllTemplatesAsync();
    Task<Result<SeatLayoutTemplateResponseDto>> GetTemplateByIdAsync(int templateId);
    Task<Result<SeatLayoutTemplateResponseDto>> CreateTemplateAsync(
        CreateSeatLayoutTemplateRequestDto request
    );
    Task<Result<SeatLayoutTemplateResponseDto>> UpdateTemplateAsync(
        int templateId,
        UpdateSeatLayoutTemplateRequestDto request
    );
    Task<Result<string>> DeactivateTemplateAsync(int templateId);
    Task<Result<List<SeatLayoutTemplateListDto>>> GetTemplatesByBusTypeAsync(int busType);
}
