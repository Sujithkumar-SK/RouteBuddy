using AutoMapper;
using Kanini.RouteBuddy.Application.Dto.Vendor;
using Kanini.RouteBuddy.Application.Dto.Common;
using Kanini.RouteBuddy.Application.Dto.Admin;
using Kanini.RouteBuddy.Application.Services.Email;
using Kanini.RouteBuddy.Data.Repositories.Vendor;
using Kanini.RouteBuddy.Data.Repositories.User;
using Kanini.RouteBuddy.Data.Repositories.Buses;
using Kanini.RouteBuddy.Data.Repositories.Schedule;
using Kanini.RouteBuddy.Data.Repositories.Route;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Application.Validators;
using Kanini.RouteBuddy.Domain.Entities;
using Kanini.RouteBuddy.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Kanini.RouteBuddy.Application.Services.Vendor;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBusRepository _busRepository;
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<VendorService> _logger;

    public VendorService(IVendorRepository vendorRepository, IUserRepository userRepository, IBusRepository busRepository, IScheduleRepository scheduleRepository, IRouteRepository routeRepository, IEmailService emailService, IMapper mapper, ILogger<VendorService> logger)
    {
        _vendorRepository = vendorRepository;
        _userRepository = userRepository;
        _busRepository = busRepository;
        _scheduleRepository = scheduleRepository;
        _routeRepository = routeRepository;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<VendorResponseDto>> RegisterVendorAsync(VendorRegistrationDto dto)
    {
        try
        {
            if (!PasswordValidator.IsValidPassword(dto.Password))
                return Result.Failure<VendorResponseDto>(Error.Failure("Password.Invalid", PasswordValidator.GetPasswordRequirements()));

            if (await _vendorRepository.ExistsByEmailAsync(dto.Email))
                return Result.Failure<VendorResponseDto>(Error.Conflict("Email.Exists", VendorMessages.EmailAlreadyExists));

            if (await _userRepository.ExistsByPhoneAsync(dto.Phone))
                return Result.Failure<VendorResponseDto>(Error.Conflict("Phone.Exists", VendorMessages.PhoneAlreadyExists));

            if (await _vendorRepository.ExistsByLicenseNumberAsync(dto.BusinessLicenseNumber))
                return Result.Failure<VendorResponseDto>(Error.Conflict("License.Exists", VendorMessages.BusinessLicenseExists));

            var user = new Domain.Entities.User
            {
                Email = dto.Email,
                PasswordHash = dto.Password,
                Phone = dto.Phone,
                Role = UserRole.Vendor,
                IsActive = true,
                IsEmailVerified = true
            };

            var createdUser = await _userRepository.AddAsync(user);

            var vendor = new Domain.Entities.Vendor
            {
                UserId = createdUser.UserId,
                AgencyName = dto.AgencyName,
                OwnerName = dto.OwnerName,
                BusinessLicenseNumber = dto.BusinessLicenseNumber,
                OfficeAddress = dto.OfficeAddress,
                FleetSize = dto.FleetSize,
                TaxRegistrationNumber = dto.TaxRegistrationNumber,
                IsActive = false,
                Status = VendorStatus.PendingApproval
            };

            var createdVendor = await _vendorRepository.CreateAsync(vendor);
            var response = _mapper.Map<VendorResponseDto>(createdVendor);
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (DbUpdateException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Unexpected.Error", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<VendorResponseDto>> GetVendorByIdAsync(int vendorId)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<VendorResponseDto>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            var response = _mapper.Map<VendorResponseDto>(vendor);
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Unexpected.Error", VendorMessages.UnexpectedError));
        }
    }

    public async Task<PagedResultDto<VendorResponseDto>> GetAllVendorsAsync(int pageNumber, int pageSize)
    {
        try
        {
            var vendors = await _vendorRepository.GetAllAsync(pageNumber, pageSize);
            var totalCount = await _vendorRepository.GetTotalCountAsync();
            
            var vendorDtos = _mapper.Map<IEnumerable<VendorResponseDto>>(vendors);

            return new PagedResultDto<VendorResponseDto>
            {
                Data = vendorDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (SqlException)
        {
            throw new InvalidOperationException(VendorMessages.DatabaseError);
        }
        catch (Exception)
        {
            throw new InvalidOperationException(VendorMessages.UnexpectedError);
        }
    }

    public async Task<Result<VendorResponseDto>> UpdateVendorAsync(int vendorId, VendorRegistrationDto dto)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null)
            return Result.Failure<VendorResponseDto>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

        _mapper.Map(dto, vendor);
        var updatedVendor = await _vendorRepository.UpdateAsync(vendor);
        var response = _mapper.Map<VendorResponseDto>(updatedVendor);
        
        return Result.Success(response);
    }

    public async Task<Result<bool>> DeleteVendorAsync(int vendorId)
    {
        var deleted = await _vendorRepository.DeleteAsync(vendorId);
        if (!deleted)
            return Result.Failure<bool>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

        return Result.Success(true);
    }

    public async Task<Result<VendorResponseDto>> ApproveVendorAsync(int vendorId)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<VendorResponseDto>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            if (vendor.Status != VendorStatus.PendingApproval)
                return Result.Failure<VendorResponseDto>(Error.Failure("Vendor.InvalidStatus", VendorMessages.InvalidVendorStatus));

            vendor.Status = VendorStatus.Active;
            vendor.IsActive = true;
            
            var updatedVendor = await _vendorRepository.UpdateAsync(vendor);
            var response = _mapper.Map<VendorResponseDto>(updatedVendor);
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (DbUpdateException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Unexpected.Error", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> RejectVendorAsync(int vendorId)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null)
            return Result.Failure<bool>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

        if (vendor.Status != VendorStatus.PendingApproval)
            return Result.Failure<bool>(Error.Failure("Vendor.InvalidStatus", VendorMessages.InvalidVendorStatus));

        vendor.Status = VendorStatus.Rejected;
        vendor.IsActive = false;
        
        await _vendorRepository.UpdateAsync(vendor);
        return Result.Success(true);
    }

    public async Task<PagedResultDto<VendorResponseDto>> GetPendingVendorsAsync(int pageNumber, int pageSize)
    {
        try
        {
            var vendors = await _vendorRepository.GetPendingVendorsAsync(pageNumber, pageSize);
            var totalCount = await _vendorRepository.GetPendingVendorsCountAsync();
            
            var vendorDtos = _mapper.Map<IEnumerable<VendorResponseDto>>(vendors);

            return new PagedResultDto<VendorResponseDto>
            {
                Data = vendorDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (SqlException)
        {
            throw new InvalidOperationException(VendorMessages.DatabaseError);
        }
        catch (Exception)
        {
            throw new InvalidOperationException(VendorMessages.UnexpectedError);
        }
    }

    public async Task<Result<VendorResponseDto>> UpdateVendorProfileAsync(int vendorId, UpdateVendorProfileDto dto)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<VendorResponseDto>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            if (vendor.Status != VendorStatus.Active)
                return Result.Failure<VendorResponseDto>(Error.Failure("Vendor.NotActive", VendorMessages.OnlyActiveVendorsCanUpdate));

            // Check email uniqueness (excluding current vendor)
            if (vendor.User.Email != dto.Email && await _userRepository.ExistsByEmailAsync(dto.Email))
                return Result.Failure<VendorResponseDto>(Error.Conflict("Email.Exists", VendorMessages.EmailAlreadyExists));

            // Check phone uniqueness (excluding current vendor)
            if (vendor.User.Phone != dto.Phone && await _userRepository.ExistsByPhoneAsync(dto.Phone))
                return Result.Failure<VendorResponseDto>(Error.Conflict("Phone.Exists", VendorMessages.PhoneAlreadyExists));

            // Update user details
            vendor.User.Email = dto.Email;
            vendor.User.Phone = dto.Phone;

            // Update vendor details
            vendor.AgencyName = dto.AgencyName;
            vendor.OwnerName = dto.OwnerName;
            vendor.OfficeAddress = dto.OfficeAddress;
            vendor.FleetSize = dto.FleetSize;

            var updatedVendor = await _vendorRepository.UpdateAsync(vendor);
            var response = _mapper.Map<VendorResponseDto>(updatedVendor);
            
            return Result.Success(response);
        }
        catch (SqlException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (DbUpdateException)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception)
        {
            return Result.Failure<VendorResponseDto>(Error.Failure("Unexpected.Error", VendorMessages.UnexpectedError));
        }
    }

    public async Task<VendorDashboardSummaryDto> GetVendorDashboardSummaryAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting dashboard summary for vendor: {VendorId}", vendorId);
            
            var dashboardData = await _vendorRepository.GetDashboardSummaryAsync(vendorId);
            
            _logger.LogInformation("Successfully retrieved dashboard data for vendor: {VendorId}", vendorId);
            
            return new VendorDashboardSummaryDto
            {
                TotalBuses = dashboardData.TotalBuses,
                ActiveBuses = dashboardData.ActiveBuses,
                PendingBuses = dashboardData.PendingBuses,
                TotalRoutes = dashboardData.TotalRoutes,
                TotalSchedules = dashboardData.TotalSchedules,
                UpcomingSchedules = dashboardData.UpcomingSchedules,
                VendorStatus = dashboardData.VendorStatus,
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error getting dashboard summary for vendor: {VendorId}", vendorId);
            throw new InvalidOperationException(VendorMessages.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting dashboard summary for vendor: {VendorId}", vendorId);
            throw new InvalidOperationException(VendorMessages.UnexpectedError);
        }
    }

    public async Task<Result<List<AdminVendorDTO>>> FilterVendorsAsync(string? searchName, bool? isActive, int? status)
    {
        try
        {
            var vendors = await _vendorRepository.FilterVendorsAsync(searchName, isActive, status);
            var response = _mapper.Map<List<AdminVendorDTO>>(vendors);
            return Result.Success(response);
        }
        catch (Exception)
        {
            return Result.Failure<List<AdminVendorDTO>>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> ApproveVendorWithEmailAsync(int vendorId)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<bool>(
                    Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            // Update vendor status
            vendor.Status = VendorStatus.Active;
            vendor.IsActive = true;
            vendor.UpdatedBy = "Admin";
            vendor.UpdatedOn = DateTime.UtcNow;
            
            // Update vendor without touching user entity
            await _vendorRepository.UpdateVendorOnlyAsync(vendor);
            
            // Separately update user IsActive status
            await _userRepository.UpdateUserActiveStatusAsync(vendor.UserId, true);
            
            // Update vendor documents to verified status
            await _vendorRepository.UpdateVendorDocumentsStatusAsync(vendorId, DocumentStatus.Verified, "Admin");
            
            // Send approval email
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendVendorApprovalEmailAsync(vendor.User.Email, vendor.OwnerName);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send vendor approval email to {Email}", vendor.User.Email);
                }
            });
            
            return Result.Success(true);
        }
        catch (Exception)
        {
            return Result.Failure<bool>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> RejectVendorWithReasonAsync(int vendorId, string rejectionReason)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<bool>(
                    Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            vendor.Status = VendorStatus.Rejected;
            vendor.IsActive = false;
            vendor.UpdatedBy = "Admin";
            vendor.UpdatedOn = DateTime.UtcNow;
            
            // Keep user account inactive for rejected vendors
            vendor.User.IsActive = false;
            vendor.User.UpdatedBy = "Admin";
            vendor.User.UpdatedOn = DateTime.UtcNow;
            
            await _vendorRepository.UpdateAsync(vendor);
            
            // Send rejection email
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendVendorRejectionEmailAsync(vendor.User.Email, vendor.OwnerName, rejectionReason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send vendor rejection email to {Email}", vendor.User.Email);
                }
            });
            
            return Result.Success(true);
        }
        catch (Exception)
        {
            return Result.Failure<bool>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> DeactivateVendorWithReasonAsync(int vendorId, string reason)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<bool>(
                    Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            vendor.IsActive = false;
            vendor.UpdatedBy = "Admin";
            vendor.UpdatedOn = DateTime.UtcNow;
            
            // Deactivate user account as well
            vendor.User.IsActive = false;
            vendor.User.UpdatedBy = "Admin";
            vendor.User.UpdatedOn = DateTime.UtcNow;
            
            await _vendorRepository.UpdateAsync(vendor);
            return Result.Success(true);
        }
        catch (Exception)
        {
            return Result.Failure<bool>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<bool>> ReactivateVendorWithReasonAsync(int vendorId, string reason)
    {
        try
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null)
                return Result.Failure<bool>(
                    Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            vendor.IsActive = true;
            vendor.UpdatedBy = "Admin";
            vendor.UpdatedOn = DateTime.UtcNow;
            
            // Reactivate user account as well
            vendor.User.IsActive = true;
            vendor.User.UpdatedBy = "Admin";
            vendor.User.UpdatedOn = DateTime.UtcNow;
            
            await _vendorRepository.UpdateAsync(vendor);
            return Result.Success(true);
        }
        catch (Exception)
        {
            return Result.Failure<bool>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<PagedResultDto<AdminVendorDTO>> GetPendingVendorsForAdminAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation("Getting pending vendors for admin - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            
            var vendors = await _vendorRepository.GetPendingVendorsAsync(pageNumber, pageSize);
            _logger.LogInformation("Retrieved {Count} pending vendors from repository", vendors?.Count() ?? 0);
            
            var totalCount = await _vendorRepository.GetPendingVendorsCountAsync();
            _logger.LogInformation("Total pending vendors count: {TotalCount}", totalCount);
            
            var vendorDtos = _mapper.Map<List<AdminVendorDTO>>(vendors);
            _logger.LogInformation("Mapped {Count} vendors to AdminVendorDTO", vendorDtos?.Count() ?? 0);

            return new PagedResultDto<AdminVendorDTO>
            {
                Data = vendorDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error getting pending vendors for admin");
            throw new InvalidOperationException(VendorMessages.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting pending vendors for admin");
            throw new InvalidOperationException(VendorMessages.UnexpectedError);
        }
    }

    public async Task<PagedResultDto<AdminVendorDTO>> GetAllVendorsForAdminAsync(int pageNumber, int pageSize)
    {
        try
        {
            _logger.LogInformation("Getting all vendors for admin - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            
            var vendors = await _vendorRepository.GetAllAsync(pageNumber, pageSize);
            _logger.LogInformation("Retrieved {Count} vendors from repository", vendors?.Count() ?? 0);
            
            var totalCount = await _vendorRepository.GetTotalCountAsync();
            _logger.LogInformation("Total vendors count: {TotalCount}", totalCount);
            
            var vendorDtos = _mapper.Map<List<AdminVendorDTO>>(vendors);
            _logger.LogInformation("Mapped {Count} vendors to AdminVendorDTO", vendorDtos?.Count() ?? 0);

            return new PagedResultDto<AdminVendorDTO>
            {
                Data = vendorDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error getting all vendors for admin");
            throw new InvalidOperationException(VendorMessages.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting all vendors for admin");
            throw new InvalidOperationException(VendorMessages.UnexpectedError);
        }
    }

    public async Task<Result<VendorApprovalDTO>> GetVendorForApprovalAsync(int vendorId)
    {
        try
        {
            var vendorWithDocuments = await _vendorRepository.GetVendorForApprovalAsync(vendorId);
            if (vendorWithDocuments == null)
                return Result.Failure<VendorApprovalDTO>(
                    Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));

            var response = _mapper.Map<VendorApprovalDTO>(vendorWithDocuments);
            return Result.Success(response);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error getting vendor for approval: {VendorId}", vendorId);
            return Result.Failure<VendorApprovalDTO>(
                Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting vendor for approval: {VendorId}", vendorId);
            return Result.Failure<VendorApprovalDTO>(
                Error.Failure("Vendor.UnexpectedError", VendorMessages.UnexpectedError));
        }
    }

    public async Task<Result<VendorResponseDto>> GetVendorByUserIdAsync(int userId)
    {
        try
        {
            _logger.LogInformation("Getting vendor by user ID: {UserId}", userId);
            
            var vendor = await _vendorRepository.GetByUserIdAsync(userId);
            if (vendor == null)
            {
                _logger.LogWarning("Vendor not found for user ID: {UserId}", userId);
                return Result.Failure<VendorResponseDto>(Error.NotFound("Vendor.NotFound", VendorMessages.VendorNotFound));
            }

            var response = _mapper.Map<VendorResponseDto>(vendor);
            _logger.LogInformation("Successfully retrieved vendor {VendorId} for user {UserId}", vendor.VendorId, userId);
            
            return Result.Success(response);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "SQL error getting vendor by user ID: {UserId}", userId);
            return Result.Failure<VendorResponseDto>(Error.Failure("Database.Error", VendorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error getting vendor by user ID: {UserId}", userId);
            return Result.Failure<VendorResponseDto>(Error.Failure("Unexpected.Error", VendorMessages.UnexpectedError));
        }
    }
}