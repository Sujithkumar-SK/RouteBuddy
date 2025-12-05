using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Utility;
using Microsoft.Extensions.Logging;
using Kanini.RouteBuddy.Data.Repositories.VendorAnalytics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kanini.RouteBuddy.Application.Services.Vendor;

public class VendorAnalyticsService : IVendorAnalyticsService
{
    private readonly IVendorAnalyticsRepository _repository;
    private readonly ILogger<VendorAnalyticsService> _logger;

    public VendorAnalyticsService(IVendorAnalyticsRepository repository, ILogger<VendorAnalyticsService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<object>> GetRevenueAnalyticsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.RevenueAnalyticsRetrieved, vendorId);
            return await _repository.GetRevenueAnalyticsAsync(vendorId);
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetPerformanceMetricsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Service: Getting performance metrics for vendor {VendorId}", vendorId);
            var result = await _repository.GetPerformanceMetricsAsync(vendorId);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Service: Successfully retrieved performance metrics");
                return result;
            }
            else
            {
                _logger.LogError("Service: Repository returned failure: {Error}", result.Error?.Description);
                return result;
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.DatabaseError, VendorAnalyticsMessages.ErrorMessages.DatabaseError));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetFleetStatusAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.FleetStatusRetrieved, vendorId);
            return await _repository.GetFleetStatusAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<List<object>>> GetNotificationsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.NotificationsRetrieved, vendorId);
            return await _repository.GetNotificationsAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<List<object>>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetMaintenanceScheduleAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.MaintenanceScheduleRetrieved, vendorId);
            return await _repository.GetMaintenanceScheduleAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetRecentBookingsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.RecentBookingsRetrieved, vendorId);
            return await _repository.GetRecentBookingsAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetQuickStatsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.QuickStatsRetrieved, vendorId);
            return await _repository.GetQuickStatsAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    public async Task<Result<object>> GetAlertsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation(VendorAnalyticsMessages.LogMessages.AlertsRetrieved, vendorId);
            return await _repository.GetAlertsAsync(vendorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, VendorAnalyticsMessages.LogMessages.AnalyticsOperationFailed, vendorId, ex.Message);
            return Result.Failure<object>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }


}