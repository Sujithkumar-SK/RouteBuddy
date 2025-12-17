using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Bus;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Common;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Extensions.Logging;
using BusEntity = Kanini.RouteBuddy.Domain.Entities.Bus;

namespace Kanini.RouteBuddy.Application.Services.Buses;

public class BusService : IBusService
{
    private readonly IBusRepository _busRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BusService> _logger;
    private readonly BlobService _blobService;

    public BusService(IBusRepository busRepository, IVendorRepository vendorRepository, IMapper mapper, ILogger<BusService> logger, BlobService blobService)
    {
        _busRepository = busRepository;
        _vendorRepository = vendorRepository;
        _mapper = mapper;
        _logger = logger;
        _blobService = blobService;
    }

    public async Task<Result<BusResponseDto>> CreateBusAsync(CreateBusDto dto, int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusCreationStarted, vendorId);

            // Clear random character strings
            if (!string.IsNullOrEmpty(dto.BusName) && dto.BusName.Length > 20 && !dto.BusName.Contains(" "))
                dto.BusName = string.Empty;
            
            if (!string.IsNullOrEmpty(dto.RegistrationNo) && dto.RegistrationNo.Length > 15)
                dto.RegistrationNo = string.Empty;

            // Business validation
            if (string.IsNullOrWhiteSpace(dto.BusName?.Trim()))
                return Result.Failure<BusResponseDto>(Error.Failure("Bus.InvalidName", "Bus name cannot be empty or whitespace"));

            if (dto.TotalSeats < 10 || dto.TotalSeats > 200)
                return Result.Failure<BusResponseDto>(Error.Failure("Bus.InvalidSeats", "Total seats must be between 10 and 200"));

            // Validate registration number format
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.RegistrationNo, @"^[A-Z]{2}[0-9]{2}[A-Z]{1,2}[0-9]{4}$"))
                return Result.Failure<BusResponseDto>(Error.Failure("Bus.InvalidRegistration", "Invalid registration number format"));

            // Check if registration number already exists
            var existsResult = await _busRepository.ExistsByRegistrationNoAsync(dto.RegistrationNo);
            if (existsResult.IsFailure)
                return Result.Failure<BusResponseDto>(existsResult.Error);

            if (existsResult.Value)
            {
                _logger.LogWarning(BusMessages.LogMessages.RegistrationExistsWarning, dto.RegistrationNo);
                return Result.Failure<BusResponseDto>(Error.Conflict(BusMessages.ErrorCodes.RegistrationExists, BusMessages.ErrorMessages.RegistrationNumberExists));
            }

            // Optional: Check if bus name already exists for this vendor (uncomment to enable)
            // var nameExistsResult = await _busRepository.ExistsByNameAndVendorAsync(dto.BusName.Trim(), vendorId);
            // if (nameExistsResult.IsFailure)
            //     return Result.Failure<BusResponseDto>(nameExistsResult.Error);
            // if (nameExistsResult.Value)
            //     return Result.Failure<BusResponseDto>(Error.Conflict("Bus.NameExists", "Bus name already exists for this vendor"));

            // Validate driver contact if provided
            if (!string.IsNullOrEmpty(dto.DriverContact) && !System.Text.RegularExpressions.Regex.IsMatch(dto.DriverContact, @"^[6-9]\d{9}$"))
                return Result.Failure<BusResponseDto>(Error.Failure("Bus.InvalidContact", "Driver contact must be a valid 10-digit mobile number"));

            // Validate registration certificate
            if (!FileValidationService.IsValidFile(dto.RegistrationCertificate, out string rcError))
                return Result.Failure<BusResponseDto>(Error.Failure("RC.Invalid", $"Registration Certificate: {rcError}"));

            // Validate document content (file header)
            if (!await FileValidationService.IsValidDocumentContentAsync(dto.RegistrationCertificate))
                return Result.Failure<BusResponseDto>(Error.Failure("RC.InvalidContent", "Registration certificate file appears to be corrupted or invalid"));

            // Upload registration certificate to Azure Blob
            var uploadResult = await _blobService.UploadFileAsync(dto.RegistrationCertificate);
            var rcPath = uploadResult.BlobUrl;

            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            
            var bus = _mapper.Map<BusEntity>(dto);
            bus.VendorId = vendorId;
            bus.IsActive = false;  // Bus should be inactive until approved
            bus.Status = BusStatus.PendingApproval;
            bus.RegistrationPath = rcPath;
            bus.SeatLayoutTemplateId = dto.SeatLayoutTemplateId; // Now mandatory
            bus.CreatedBy = vendor?.AgencyName ?? "System";
            bus.CreatedOn = DateTime.UtcNow;

            var createResult = await _busRepository.CreateAsync(bus);
            if (createResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusCreationFailed, createResult.Error.Description);
                return Result.Failure<BusResponseDto>(createResult.Error);
            }

            var response = _mapper.Map<BusResponseDto>(createResult.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";

            _logger.LogInformation(BusMessages.LogMessages.BusCreatedSuccessfully, createResult.Value.BusId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus creation failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<BusResponseDto>> GetBusByIdAsync(int busId, int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusRetrievalStarted, busId);

            var result = await _busRepository.GetByIdAsync(busId);
            if (result.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusRetrievalFailed, result.Error.Description);
                return Result.Failure<BusResponseDto>(result.Error);
            }

            // Vendor authorization check
            if (result.Value.VendorId != vendorId)
            {
                _logger.LogWarning(BusMessages.LogMessages.UnauthorizedAccessWarning, busId);
                return Result.Failure<BusResponseDto>(Error.NotFound(BusMessages.ErrorCodes.BusNotFound, BusMessages.ErrorMessages.UnauthorizedBusAccess));
            }

            var vendor = await _vendorRepository.GetByIdAsync(result.Value.VendorId);
            var response = _mapper.Map<BusResponseDto>(result.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";
            
            _logger.LogInformation(BusMessages.LogMessages.BusRetrievedSuccessfully, busId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus retrieval failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<PagedResultDto<BusResponseDto>>> GetBusesByVendorAsync(int vendorId, int pageNumber, int pageSize, BusStatus? status = null, BusType? busType = null, string? search = null)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusListRetrievalStarted, vendorId);

            var busesResult = await _busRepository.GetByVendorIdAsync(vendorId, pageNumber, pageSize, status, busType, search);
            if (busesResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusListFailed, vendorId);
                return Result.Failure<PagedResultDto<BusResponseDto>>(busesResult.Error);
            }

            var countResult = await _busRepository.GetCountByVendorIdAsync(vendorId, status, busType, search);
            if (countResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusCountFailed, vendorId);
                return Result.Failure<PagedResultDto<BusResponseDto>>(countResult.Error);
            }

            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            var busDtos = _mapper.Map<List<BusResponseDto>>(busesResult.Value);
            foreach (var dto in busDtos)
            {
                dto.VendorName = vendor?.AgencyName ?? "Unknown";
            }

            var pagedResult = new PagedResultDto<BusResponseDto>
            {
                Data = busDtos,
                TotalCount = countResult.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            _logger.LogInformation(BusMessages.LogMessages.BusesByVendorRetrievedSuccessfully, busDtos.Count, vendorId);
            return Result.Success(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus list failed: {Message}", ex.Message);
            return Result.Failure<PagedResultDto<BusResponseDto>>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<BusResponseDto>> UpdateBusAsync(int busId, UpdateBusDto dto)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusUpdateStarted, busId);

            var result = await _busRepository.GetByIdAsync(busId);
            if (result.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusUpdateFailed, busId);
                return Result.Failure<BusResponseDto>(result.Error);
            }

            var vendor = await _vendorRepository.GetByIdAsync(result.Value.VendorId);
            _mapper.Map(dto, result.Value);
            result.Value.UpdatedBy = vendor?.AgencyName ?? "System";
            result.Value.UpdatedOn = DateTime.UtcNow;
            
            var updateResult = await _busRepository.UpdateAsync(result.Value);
            if (updateResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusUpdateFailed, busId);
                return Result.Failure<BusResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<BusResponseDto>(updateResult.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";

            _logger.LogInformation(BusMessages.LogMessages.BusUpdatedSuccessfully, busId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus update failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<bool>> DeleteBusAsync(int busId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusDeleteStarted, busId);

            var result = await _busRepository.DeleteAsync(busId);
            if (result.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusDeleteFailed, busId);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation(BusMessages.LogMessages.BusDeletedSuccessfully, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus delete failed: {Message}", ex.Message);
            return Result.Failure<bool>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<BusResponseDto>> ActivateBusAsync(int busId, int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusActivationStarted, busId);

            var result = await _busRepository.GetByIdAsync(busId);
            if (result.IsFailure)
                return Result.Failure<BusResponseDto>(result.Error);

            if (result.Value.VendorId != vendorId)
            {
                _logger.LogWarning(BusMessages.LogMessages.UnauthorizedAccessWarning, busId);
                return Result.Failure<BusResponseDto>(Error.NotFound(BusMessages.ErrorCodes.BusNotFound, BusMessages.ErrorMessages.UnauthorizedBusAccess));
            }

            // Allow activation regardless of current status
            // if (result.Value.Status != BusStatus.PendingApproval)
            // {
            //     _logger.LogWarning(BusMessages.LogMessages.InvalidStatusWarning, busId);
            //     return Result.Failure<BusResponseDto>(Error.Failure(BusMessages.ErrorCodes.InvalidStatus, BusMessages.ErrorMessages.InvalidBusStatus));
            // }

            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            result.Value.Status = BusStatus.Active;
            result.Value.IsActive = true;
            result.Value.UpdatedBy = vendor?.AgencyName ?? "System";
            result.Value.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(result.Value);
            if (updateResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusActivationFailed, busId);
                return Result.Failure<BusResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<BusResponseDto>(updateResult.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";

            _logger.LogInformation(BusMessages.LogMessages.BusActivatedSuccessfully, busId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus activation failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<BusResponseDto>> DeactivateBusAsync(int busId, int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusDeactivationStarted, busId);

            var result = await _busRepository.GetByIdAsync(busId);
            if (result.IsFailure)
                return Result.Failure<BusResponseDto>(result.Error);

            if (result.Value.VendorId != vendorId)
            {
                _logger.LogWarning(BusMessages.LogMessages.UnauthorizedAccessWarning, busId);
                return Result.Failure<BusResponseDto>(Error.NotFound(BusMessages.ErrorCodes.BusNotFound, BusMessages.ErrorMessages.UnauthorizedBusAccess));
            }

            if (!result.Value.IsActive)
            {
                _logger.LogWarning(BusMessages.LogMessages.BusAlreadyInactiveWarning, busId);
                return Result.Failure<BusResponseDto>(Error.Failure(BusMessages.ErrorCodes.AlreadyInactive, BusMessages.ErrorMessages.BusAlreadyInactive));
            }

            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            result.Value.IsActive = false;
            result.Value.UpdatedBy = vendor?.AgencyName ?? "System";
            result.Value.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(result.Value);
            if (updateResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusDeactivationFailed, busId);
                return Result.Failure<BusResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<BusResponseDto>(updateResult.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";

            _logger.LogInformation(BusMessages.LogMessages.BusDeactivatedSuccessfully, busId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bus deactivation failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }



    public async Task<Result<BusResponseDto>> SetMaintenanceAsync(int busId, int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.BusMaintenanceStarted, busId);

            var result = await _busRepository.GetByIdAsync(busId);
            if (result.IsFailure)
                return Result.Failure<BusResponseDto>(result.Error);

            if (result.Value.VendorId != vendorId)
            {
                _logger.LogWarning(BusMessages.LogMessages.UnauthorizedAccessWarning, busId);
                return Result.Failure<BusResponseDto>(Error.NotFound(BusMessages.ErrorCodes.BusNotFound, BusMessages.ErrorMessages.UnauthorizedBusAccess));
            }

            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            result.Value.Status = BusStatus.Maintenance;
            result.Value.UpdatedBy = vendor?.AgencyName ?? "System";
            result.Value.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _busRepository.UpdateAsync(result.Value);
            if (updateResult.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.BusMaintenanceFailed, busId);
                return Result.Failure<BusResponseDto>(updateResult.Error);
            }

            var response = _mapper.Map<BusResponseDto>(updateResult.Value);
            response.VendorName = vendor?.AgencyName ?? "Unknown";

            _logger.LogInformation(BusMessages.LogMessages.BusSetToMaintenanceSuccessfully, busId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Set maintenance failed: {Message}", ex.Message);
            return Result.Failure<BusResponseDto>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<List<BusResponseDto>>> GetAwaitingConfirmationAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(BusMessages.LogMessages.AwaitingConfirmationStarted, vendorId);

            var result = await _busRepository.GetAwaitingConfirmationByVendorAsync(vendorId);
            if (result.IsFailure)
            {
                _logger.LogError(BusMessages.LogMessages.AwaitingConfirmationFailed, vendorId);
                return Result.Failure<List<BusResponseDto>>(result.Error);
            }

            var busDtos = _mapper.Map<List<BusResponseDto>>(result.Value);
            _logger.LogInformation(BusMessages.LogMessages.AwaitingConfirmationRetrievedSuccessfully, vendorId);
            return Result.Success(busDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Awaiting confirmation buses failed: {Message}", ex.Message);
            return Result.Failure<List<BusResponseDto>>(
                Error.Failure(BusMessages.ErrorCodes.BusUnexpectedError, BusMessages.ErrorMessages.UnexpectedError)
            );
        }
    }

    public async Task<Result<bool>> ApplyTemplateAsync(int busId, int templateId, int vendorId)
    {
        try
        {
            _logger.LogInformation(MagicStrings.LogMessages.BusTemplateApplicationStarted, busId, templateId);

            // Validate bus belongs to vendor
            var busResult = await _busRepository.GetByIdAsync(busId);
            if (busResult.IsFailure)
            {
                _logger.LogError("Bus not found for template application: {BusId}", busId);
                return Result.Failure<bool>(Error.NotFound("Bus.NotFound", "Bus not found"));
            }

            if (busResult.Value.VendorId != vendorId)
            {
                _logger.LogWarning("Unauthorized template application attempt for bus: {BusId}", busId);
                return Result.Failure<bool>(Error.NotFound("Bus.NotFound", "Bus not found"));
            }

            // Apply template
            var result = await _busRepository.ApplyTemplateAsync(busId, templateId);
            if (result.IsFailure)
            {
                _logger.LogError(MagicStrings.LogMessages.BusTemplateApplicationFailed, result.Error.Description);
                return Result.Failure<bool>(result.Error);
            }

            _logger.LogInformation(MagicStrings.LogMessages.BusTemplateApplicationCompleted, busId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Apply template service failed for bus: {BusId}", busId);
            return Result.Failure<bool>(
                Error.Failure("Template.Failed", "Template application failed")
            );
        }
    }

    public async Task<Domain.Entities.Vendor?> GetVendorByUserIdAsync(int userId)
    {
        try
        {
            var result = await _vendorRepository.GetByUserIdAsync(userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vendor by UserId: {UserId}", userId);
            return null;
        }
    }
}