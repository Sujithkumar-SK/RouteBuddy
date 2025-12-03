using AutoMapper;
using Kanini.RouteBuddy.Application.Dto;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Admin;
using Kanini.RouteBuddy.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.Admin;

public class AdminSeatLayoutService : IAdminSeatLayoutService
{
    private readonly ISeatLayoutRepository _seatLayoutRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminSeatLayoutService> _logger;

    public AdminSeatLayoutService(
        ISeatLayoutRepository seatLayoutRepository,
        IMapper mapper,
        ILogger<AdminSeatLayoutService> logger
    )
    {
        _seatLayoutRepository = seatLayoutRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<SeatLayoutTemplateListDto>>> GetAllTemplatesAsync()
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplateGetAllStarted);

            var result = await _seatLayoutRepository.GetAllTemplatesAsync();
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetAllFailed,
                    result.Error.Description
                );
                return Result.Failure<List<SeatLayoutTemplateListDto>>(result.Error);
            }

            var templateListDtos = _mapper.Map<List<SeatLayoutTemplateListDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetAllCompleted,
                templateListDtos.Count
            );
            return Result.Success(templateListDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetAllFailed,
                ex.Message
            );
            return Result.Failure<List<SeatLayoutTemplateListDto>>(
                Error.Failure(
                    "AdminSeatLayoutService.GetAllFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplateResponseDto>> GetTemplateByIdAsync(int templateId)
    {
        try
        {
            if (templateId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<SeatLayoutTemplateResponseDto>(
                    Error.Failure(
                        "AdminSeatLayoutService.InvalidId",
                        "Template ID must be greater than 0"
                    )
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdStarted,
                templateId
            );

            var result = await _seatLayoutRepository.GetTemplateWithDetailsAsync(templateId);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    result.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(result.Error);
            }

            var templateResponseDto = _mapper.Map<SeatLayoutTemplateResponseDto>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdCompleted,
                templateId
            );
            return Result.Success(templateResponseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplateResponseDto>(
                Error.Failure(
                    "AdminSeatLayoutService.GetByIdFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplateResponseDto>> CreateTemplateAsync(
        CreateSeatLayoutTemplateRequestDto request
    )
    {
        try
        {
            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationStarted,
                request.TemplateName
            );

            var validationResult = ValidateCreateRequest(request);
            if (validationResult.IsFailure)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<SeatLayoutTemplateResponseDto>(validationResult.Error);
            }

            var template = _mapper.Map<SeatLayoutTemplate>(request);
            var seatDetails = _mapper.Map<List<SeatLayoutDetail>>(request.SeatDetails);

            var result = await _seatLayoutRepository.CreateTemplateAsync(template, seatDetails);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateCreationFailed,
                    result.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(result.Error);
            }

            var createdTemplateResult = await _seatLayoutRepository.GetTemplateWithDetailsAsync(
                result.Value.SeatLayoutTemplateId
            );
            if (createdTemplateResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    createdTemplateResult.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(createdTemplateResult.Error);
            }

            var responseDto = _mapper.Map<SeatLayoutTemplateResponseDto>(
                createdTemplateResult.Value
            );

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateCreationCompleted,
                result.Value.SeatLayoutTemplateId
            );
            return Result.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateCreationFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplateResponseDto>(
                Error.Failure(
                    "AdminSeatLayoutService.CreateFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<SeatLayoutTemplateResponseDto>> UpdateTemplateAsync(
        int templateId,
        UpdateSeatLayoutTemplateRequestDto request
    )
    {
        try
        {
            if (templateId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<SeatLayoutTemplateResponseDto>(
                    Error.Failure(
                        "AdminSeatLayoutService.InvalidId",
                        "Template ID must be greater than 0"
                    )
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateStarted,
                templateId
            );

            // Check if template exists
            var existingTemplateResult = await _seatLayoutRepository.GetTemplateByIdAsync(
                templateId
            );
            if (existingTemplateResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                    existingTemplateResult.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(existingTemplateResult.Error);
            }

            // Business validation
            var validationResult = ValidateUpdateRequest(request);
            if (validationResult.IsFailure)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<SeatLayoutTemplateResponseDto>(validationResult.Error);
            }

            // Map DTOs to entities (service responsibility)
            var template = _mapper.Map<SeatLayoutTemplate>(request);
            template.SeatLayoutTemplateId = templateId;
            template.CreatedBy = existingTemplateResult.Value.CreatedBy;
            template.CreatedOn = existingTemplateResult.Value.CreatedOn;
            var seatDetails = _mapper.Map<List<SeatLayoutDetail>>(request.SeatDetails);

            // Update template via repository
            var result = await _seatLayoutRepository.UpdateTemplateAsync(template, seatDetails);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                    result.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(result.Error);
            }

            // Get updated template with details
            var updatedTemplateResult = await _seatLayoutRepository.GetTemplateWithDetailsAsync(
                templateId
            );
            if (updatedTemplateResult.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateGetByIdFailed,
                    updatedTemplateResult.Error.Description
                );
                return Result.Failure<SeatLayoutTemplateResponseDto>(updatedTemplateResult.Error);
            }

            var responseDto = _mapper.Map<SeatLayoutTemplateResponseDto>(
                updatedTemplateResult.Value
            );

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateCompleted,
                templateId
            );
            return Result.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateUpdateFailed,
                ex.Message
            );
            return Result.Failure<SeatLayoutTemplateResponseDto>(
                Error.Failure(
                    "AdminSeatLayoutService.UpdateFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    public async Task<Result<string>> DeactivateTemplateAsync(int templateId)
    {
        try
        {
            if (templateId <= 0)
            {
                _logger.LogWarning(MagicStrings.LogMessages.ValidationFailed);
                return Result.Failure<string>(
                    Error.Failure(
                        "AdminSeatLayoutService.InvalidId",
                        "Template ID must be greater than 0"
                    )
                );
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationStarted,
                templateId
            );

            var result = await _seatLayoutRepository.DeactivateTemplateAsync(templateId);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplateDeactivationFailed,
                    result.Error.Description
                );
                return Result.Failure<string>(result.Error);
            }

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationCompleted,
                templateId
            );
            return Result.Success("Seat layout template deactivated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplateDeactivationFailed,
                ex.Message
            );
            return Result.Failure<string>(
                Error.Failure(
                    "AdminSeatLayoutService.DeactivationFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }

    private Result ValidateCreateRequest(CreateSeatLayoutTemplateRequestDto request)
    {
        // Validate seat details count matches total seats
        if (request.SeatDetails.Count != request.TotalSeats)
        {
            return Result.Failure(
                Error.Failure(
                    "SeatLayoutTemplate.SeatDetailsMismatch",
                    MagicStrings.ErrorMessages.SeatLayoutTemplateSeatDetailsMismatch
                )
            );
        }

        // Validate unique seat numbers
        var seatNumbers = request.SeatDetails.Select(s => s.SeatNumber).ToList();
        if (seatNumbers.Count != seatNumbers.Distinct().Count())
        {
            return Result.Failure(
                Error.Failure(
                    "SeatLayoutTemplate.DuplicateSeatNumbers",
                    MagicStrings.ErrorMessages.SeatLayoutTemplateDuplicateSeatNumber
                )
            );
        }

        // Validate row and column numbers
        foreach (var seat in request.SeatDetails)
        {
            if (seat.RowNumber <= 0 || seat.ColumnNumber <= 0)
            {
                return Result.Failure(
                    Error.Failure(
                        "SeatLayoutTemplate.InvalidRowColumn",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateInvalidRowColumn
                    )
                );
            }
        }

        return Result.Success();
    }

    private Result ValidateUpdateRequest(UpdateSeatLayoutTemplateRequestDto request)
    {
        // Validate seat details count matches total seats
        if (request.SeatDetails.Count != request.TotalSeats)
        {
            return Result.Failure(
                Error.Failure(
                    "SeatLayoutTemplate.SeatDetailsMismatch",
                    MagicStrings.ErrorMessages.SeatLayoutTemplateSeatDetailsMismatch
                )
            );
        }

        // Validate unique seat numbers
        var seatNumbers = request.SeatDetails.Select(s => s.SeatNumber).ToList();
        if (seatNumbers.Count != seatNumbers.Distinct().Count())
        {
            return Result.Failure(
                Error.Failure(
                    "SeatLayoutTemplate.DuplicateSeatNumbers",
                    MagicStrings.ErrorMessages.SeatLayoutTemplateDuplicateSeatNumber
                )
            );
        }

        // Validate row and column numbers
        foreach (var seat in request.SeatDetails)
        {
            if (seat.RowNumber <= 0 || seat.ColumnNumber <= 0)
            {
                return Result.Failure(
                    Error.Failure(
                        "SeatLayoutTemplate.InvalidRowColumn",
                        MagicStrings.ErrorMessages.SeatLayoutTemplateInvalidRowColumn
                    )
                );
            }
        }

        return Result.Success();
    }

    public async Task<Result<List<SeatLayoutTemplateListDto>>> GetTemplatesByBusTypeAsync(int busType)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeStarted, busType);

            var result = await _seatLayoutRepository.GetTemplatesByBusTypeAsync(busType);
            if (result.IsFailure)
            {
                _logger.LogError(
                    MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeFailed,
                    result.Error.Description
                );
                return Result.Failure<List<SeatLayoutTemplateListDto>>(result.Error);
            }

            var templateListDtos = _mapper.Map<List<SeatLayoutTemplateListDto>>(result.Value);

            _logger.LogInformation(
                MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeCompleted,
                templateListDtos.Count
            );
            return Result.Success(templateListDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                MagicStrings.LogMessages.SeatLayoutTemplatesByBusTypeFailed,
                ex.Message
            );
            return Result.Failure<List<SeatLayoutTemplateListDto>>(
                Error.Failure(
                    "AdminSeatLayoutService.GetByBusTypeFailed",
                    MagicStrings.ErrorMessages.UnexpectedError
                )
            );
        }
    }
}
