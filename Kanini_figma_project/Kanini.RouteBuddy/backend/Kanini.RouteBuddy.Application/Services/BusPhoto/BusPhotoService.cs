using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.BusPhoto;
using Kanini.RouteBuddy.Data.Repositories.BusPhoto;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Application.Services.BusPhoto;

public class BusPhotoService : IBusPhotoService
{
    private readonly IBusPhotoRepository _repository;
    private readonly IBusRepository _busRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BusPhotoService> _logger;
    private readonly BlobService _blobService;
    private const int MaxPhotosPerBus = 10;
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB

    public BusPhotoService(IBusPhotoRepository repository, IBusRepository busRepository, IVendorRepository vendorRepository, IMapper mapper, ILogger<BusPhotoService> logger, BlobService blobService)
    {
        _repository = repository;
        _busRepository = busRepository;
        _vendorRepository = vendorRepository;
        _mapper = mapper;
        _logger = logger;
        _blobService = blobService;
    }

    public async Task<Result<BusPhotoDto>> CreateAsync(CreateBusPhotoDto createDto)
    {
        try
        {
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoCreationStarted, createDto.BusId);

            // Validate bus exists
            var busExistsResult = await _busRepository.ExistsAsync(createDto.BusId);
            if (busExistsResult.IsFailure)
                return Result.Failure<BusPhotoDto>(busExistsResult.Error);

            if (!busExistsResult.Value)
                return Result.Failure<BusPhotoDto>(Error.NotFound("Bus.NotFound", "Bus not found"));

            // Validate photo count limit
            var photoCountResult = await _repository.GetCountByBusIdAsync(createDto.BusId);
            if (photoCountResult.IsFailure)
                return Result.Failure<BusPhotoDto>(photoCountResult.Error);

            if (photoCountResult.Value >= MaxPhotosPerBus)
                return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoLimitExceeded, $"Maximum {MaxPhotosPerBus} photos allowed per bus"));

            // Validate file
            if (!FileValidationService.IsValidFile(createDto.Photo, out string fileError))
                return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoInvalidFile, fileError));

            // Validate file size
            if (createDto.Photo.Length > MaxFileSizeBytes)
                return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoFileTooLarge, BusPhotoMessages.ErrorMessages.FileTooLarge));

            // Validate image content
            if (!await FileValidationService.IsValidDocumentContentAsync(createDto.Photo))
                return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoInvalidContent, BusPhotoMessages.ErrorMessages.InvalidContent));

            // Upload to Azure Blob Storage
            var uploadResult = await _blobService.UploadFileAsync(createDto.Photo);
            var imagePath = uploadResult.BlobUrl;

            // Get bus to find vendor
            var busResult = await _busRepository.GetByIdAsync(createDto.BusId);
            var vendor = busResult.IsSuccess ? await _vendorRepository.GetByIdAsync(busResult.Value.VendorId) : null;
            var createdBy = vendor?.AgencyName ?? "System";

            var busPhoto = new Domain.Entities.BusPhoto
            {
                BusId = createDto.BusId,
                ImagePath = imagePath,
                Caption = createDto.Caption?.Trim(),
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };

            var createResult = await _repository.CreateAsync(busPhoto);
            if (createResult.IsFailure)
            {
                _logger.LogError("Failed to create bus photo: {Error}", createResult.Error.Description);
                return Result.Failure<BusPhotoDto>(createResult.Error);
            }

            var response = _mapper.Map<BusPhotoDto>(createResult.Value);
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoCreatedSuccessfully, createResult.Value.BusPhotoId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating bus photo: {Message}", ex.Message);
            return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUnexpectedError, BusPhotoMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<BusPhotoDto>> GetByIdAsync(int busPhotoId)
    {
        try
        {
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoRetrievalStarted, busPhotoId);

            var result = await _repository.GetByIdAsync(busPhotoId);
            if (result.IsFailure)
                return Result.Failure<BusPhotoDto>(result.Error);

            var response = _mapper.Map<BusPhotoDto>(result.Value);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving bus photo: {Message}", ex.Message);
            return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUnexpectedError, BusPhotoMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<IEnumerable<BusPhotoDto>>> GetByBusIdAsync(int busId)
    {
        try
        {
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotosRetrievalStarted, busId);

            var result = await _repository.GetByBusIdAsync(busId);
            if (result.IsFailure)
                return Result.Failure<IEnumerable<BusPhotoDto>>(result.Error);

            var response = _mapper.Map<IEnumerable<BusPhotoDto>>(result.Value);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving bus photos: {Message}", ex.Message);
            return Result.Failure<IEnumerable<BusPhotoDto>>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUnexpectedError, BusPhotoMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<BusPhotoDto>> UpdateAsync(int busPhotoId, UpdateBusPhotoDto updateDto)
    {
        try
        {
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoUpdateStarted, busPhotoId);

            var getResult = await _repository.GetByIdAsync(busPhotoId);
            if (getResult.IsFailure)
                return Result.Failure<BusPhotoDto>(getResult.Error);

            var busPhoto = getResult.Value;
            busPhoto.Caption = updateDto.Caption?.Trim();
            // Get bus to find vendor
            var busResult = await _busRepository.GetByIdAsync(busPhoto.BusId);
            var vendor = busResult.IsSuccess ? await _vendorRepository.GetByIdAsync(busResult.Value.VendorId) : null;
            busPhoto.UpdatedBy = vendor?.AgencyName ?? "System";
            busPhoto.UpdatedOn = DateTime.UtcNow;

            var updateResult = await _repository.UpdateAsync(busPhoto);
            if (updateResult.IsFailure)
                return Result.Failure<BusPhotoDto>(updateResult.Error);

            var response = _mapper.Map<BusPhotoDto>(updateResult.Value);
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoUpdatedSuccessfully, busPhotoId);
            return Result.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating bus photo: {Message}", ex.Message);
            return Result.Failure<BusPhotoDto>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUnexpectedError, BusPhotoMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> DeleteAsync(int busPhotoId)
    {
        try
        {
            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoDeleteStarted, busPhotoId);

            var result = await _repository.DeleteAsync(busPhotoId);
            if (result.IsFailure)
                return Result.Failure<bool>(result.Error);

            _logger.LogInformation(BusPhotoMessages.LogMessages.BusPhotoDeletedSuccessfully, busPhotoId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting bus photo: {Message}", ex.Message);
            return Result.Failure<bool>(Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUnexpectedError, BusPhotoMessages.ErrorMessages.UnexpectedError));
        }
    }
}