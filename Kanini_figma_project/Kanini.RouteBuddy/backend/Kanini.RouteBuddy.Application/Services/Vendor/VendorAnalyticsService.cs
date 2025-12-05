using Kanini.RouteBuddy.Application.Dto.Vendor;
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

    public async Task<Result<VendorAnalyticsDto>> GetCompleteAnalyticsAsync(int vendorId)
    {
        try
        {
            _logger.LogInformation("Getting complete analytics for vendor {VendorId}", vendorId);

            // Fetch all analytics data concurrently
            var tasks = new Task[8]
            {
                _repository.GetRevenueAnalyticsAsync(vendorId),
                _repository.GetPerformanceMetricsAsync(vendorId),
                _repository.GetFleetStatusAsync(vendorId),
                _repository.GetQuickStatsAsync(vendorId),
                _repository.GetRecentBookingsAsync(vendorId),
                _repository.GetNotificationsAsync(vendorId),
                _repository.GetAlertsAsync(vendorId),
                _repository.GetMaintenanceScheduleAsync(vendorId)
            };

            await Task.WhenAll(tasks);

            // Extract results
            var revenueResult = ((Task<Result<object>>)tasks[0]).Result;
            var performanceResult = ((Task<Result<object>>)tasks[1]).Result;
            var fleetResult = ((Task<Result<object>>)tasks[2]).Result;
            var quickStatsResult = ((Task<Result<object>>)tasks[3]).Result;
            var bookingsResult = ((Task<Result<object>>)tasks[4]).Result;
            var notificationsResult = ((Task<Result<List<object>>>)tasks[5]).Result;
            var alertsResult = ((Task<Result<object>>)tasks[6]).Result;
            var maintenanceResult = ((Task<Result<object>>)tasks[7]).Result;

            // Map to DTOs
            var analytics = new VendorAnalyticsDto
            {
                RevenueAnalytics = MapRevenueAnalytics(revenueResult.IsSuccess ? revenueResult.Value : null),
                PerformanceMetrics = MapPerformanceMetrics(performanceResult.IsSuccess ? performanceResult.Value : null),
                FleetStatus = MapFleetStatus(fleetResult.IsSuccess ? fleetResult.Value : null),
                QuickStats = MapQuickStats(quickStatsResult.IsSuccess ? quickStatsResult.Value : null),
                RecentBookings = MapRecentBookings(bookingsResult.IsSuccess ? bookingsResult.Value : null),
                Notifications = MapNotifications(notificationsResult.IsSuccess ? notificationsResult.Value : null),
                Alerts = MapAlerts(alertsResult.IsSuccess ? alertsResult.Value : null),
                MaintenanceSchedule = MapMaintenanceSchedule(maintenanceResult.IsSuccess ? maintenanceResult.Value : null)
            };

            _logger.LogInformation("Successfully retrieved complete analytics for vendor {VendorId}", vendorId);
            return Result.Success(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting complete analytics for vendor {VendorId}", vendorId);
            return Result.Failure<VendorAnalyticsDto>(Error.Failure(VendorAnalyticsMessages.ErrorCodes.UnexpectedError, VendorAnalyticsMessages.ErrorMessages.UnexpectedError));
        }
    }

    private RevenueAnalyticsDto MapRevenueAnalytics(object? data)
    {
        if (data == null) return new RevenueAnalyticsDto();
        
        try
        {
            var type = data.GetType();
            var totalRevenue = type.GetProperty("TotalRevenue")?.GetValue(data) ?? 0m;
            var monthlyRevenue = type.GetProperty("MonthlyRevenue")?.GetValue(data) ?? 0m;
            var weeklyRevenue = type.GetProperty("WeeklyRevenue")?.GetValue(data) ?? 0m;
            
            return new RevenueAnalyticsDto
            {
                TotalRevenue = Convert.ToDecimal(totalRevenue),
                MonthlyRevenue = Convert.ToDecimal(monthlyRevenue),
                WeeklyRevenue = Convert.ToDecimal(weeklyRevenue)
            };
        }
        catch
        {
            return new RevenueAnalyticsDto();
        }
    }

    private PerformanceMetricsDto MapPerformanceMetrics(object? data)
    {
        if (data == null) return new PerformanceMetricsDto();
        
        try
        {
            var type = data.GetType();
            var monthlyBookings = type.GetProperty("MonthlyBookings")?.GetValue(data) ?? 0;
            var onTimePerformance = type.GetProperty("OnTimePerformance")?.GetValue(data) ?? 0.0;
            
            return new PerformanceMetricsDto
            {
                MonthlyBookings = Convert.ToInt32(monthlyBookings),
                OnTimePerformance = Convert.ToDouble(onTimePerformance)
            };
        }
        catch
        {
            return new PerformanceMetricsDto();
        }
    }

    private FleetStatusDto MapFleetStatus(object? data)
    {
        if (data == null) return new FleetStatusDto();
        
        try
        {
            var type = data.GetType();
            var totalBuses = type.GetProperty("TotalBuses")?.GetValue(data) ?? 0;
            var activeBuses = type.GetProperty("ActiveBuses")?.GetValue(data) ?? 0;
            var maintenanceBuses = type.GetProperty("MaintenanceBuses")?.GetValue(data) ?? 0;
            var idleBuses = type.GetProperty("IdleBuses")?.GetValue(data) ?? 0;
            
            return new FleetStatusDto
            {
                TotalBuses = Convert.ToInt32(totalBuses),
                ActiveBuses = Convert.ToInt32(activeBuses),
                MaintenanceBuses = Convert.ToInt32(maintenanceBuses),
                IdleBuses = Convert.ToInt32(idleBuses)
            };
        }
        catch
        {
            return new FleetStatusDto();
        }
    }

    private QuickStatsDto MapQuickStats(object? data)
    {
        if (data == null) return new QuickStatsDto();
        
        try
        {
            var type = data.GetType();
            var totalBookings = type.GetProperty("TotalBookings")?.GetValue(data) ?? 0;
            var activeRoutes = type.GetProperty("ActiveRoutes")?.GetValue(data) ?? 0;
            var totalRevenue = type.GetProperty("TotalRevenue")?.GetValue(data) ?? 0m;
            
            return new QuickStatsDto
            {
                TotalBookings = Convert.ToInt32(totalBookings),
                ActiveRoutes = Convert.ToInt32(activeRoutes),
                TotalRevenue = Convert.ToDecimal(totalRevenue)
            };
        }
        catch
        {
            return new QuickStatsDto();
        }
    }

    private List<RecentBookingDto> MapRecentBookings(object? data)
    {
        if (data == null || !(data is List<object> bookings)) return new List<RecentBookingDto>();
        
        return bookings.Select(b =>
        {
            try
            {
                var type = b.GetType();
                var bookingId = type.GetProperty("BookingId")?.GetValue(b) ?? 0;
                var customerName = type.GetProperty("CustomerName")?.GetValue(b) ?? "";
                var route = type.GetProperty("Route")?.GetValue(b) ?? "";
                var bookingDate = type.GetProperty("BookingDate")?.GetValue(b) ?? DateTime.Now;
                var status = type.GetProperty("Status")?.GetValue(b) ?? "";
                
                return new RecentBookingDto
                {
                    BookingId = Convert.ToInt32(bookingId),
                    CustomerName = customerName.ToString() ?? "",
                    Route = route.ToString() ?? "",
                    BookingDate = Convert.ToDateTime(bookingDate),
                    Status = status.ToString() ?? ""
                };
            }
            catch
            {
                return new RecentBookingDto();
            }
        }).ToList();
    }

    private List<VendorNotificationDto> MapNotifications(List<object>? data)
    {
        if (data == null) return new List<VendorNotificationDto>();
        
        return data.Select(n =>
        {
            try
            {
                var type = n.GetType();
                var notificationType = type.GetProperty("Type")?.GetValue(n) ?? "";
                var message = type.GetProperty("Message")?.GetValue(n) ?? "";
                var time = type.GetProperty("Time")?.GetValue(n) ?? DateTime.Now;
                
                return new VendorNotificationDto
                {
                    Type = notificationType.ToString() ?? "",
                    Message = message.ToString() ?? "",
                    Time = Convert.ToDateTime(time)
                };
            }
            catch
            {
                return new VendorNotificationDto();
            }
        }).ToList();
    }

    private List<VendorAlertDto> MapAlerts(object? data)
    {
        if (data == null || !(data is List<object> alerts)) return new List<VendorAlertDto>();
        
        return alerts.Select(a =>
        {
            try
            {
                var type = a.GetType();
                var alertId = type.GetProperty("AlertId")?.GetValue(a) ?? 0;
                var alertType = type.GetProperty("Type")?.GetValue(a) ?? "";
                var message = type.GetProperty("Message")?.GetValue(a) ?? "";
                var severity = type.GetProperty("Severity")?.GetValue(a) ?? "";
                var createdAt = type.GetProperty("CreatedAt")?.GetValue(a) ?? DateTime.Now;
                
                return new VendorAlertDto
                {
                    AlertId = Convert.ToInt32(alertId),
                    Type = alertType.ToString() ?? "",
                    Message = message.ToString() ?? "",
                    Severity = severity.ToString() ?? "",
                    CreatedAt = Convert.ToDateTime(createdAt)
                };
            }
            catch
            {
                return new VendorAlertDto();
            }
        }).ToList();
    }

    private MaintenanceScheduleDto MapMaintenanceSchedule(object? data)
    {
        if (data == null) return new MaintenanceScheduleDto();
        
        try
        {
            var type = data.GetType();
            var upcomingMaintenanceProperty = type.GetProperty("UpcomingMaintenance")?.GetValue(data);
            var upcomingList = upcomingMaintenanceProperty as List<object> ?? new List<object>();
            
            return new MaintenanceScheduleDto
            {
                UpcomingMaintenance = upcomingList.Select(u =>
                {
                    try
                    {
                        var uType = u.GetType();
                        var busNumber = uType.GetProperty("BusNumber")?.GetValue(u) ?? "";
                        var maintenanceType = uType.GetProperty("Type")?.GetValue(u) ?? "";
                        var dueDate = uType.GetProperty("DueDate")?.GetValue(u) ?? DateTime.Now;
                        var priority = uType.GetProperty("Priority")?.GetValue(u) ?? "";
                        
                        return new UpcomingMaintenanceDto
                        {
                            BusNumber = busNumber.ToString() ?? "",
                            Type = maintenanceType.ToString() ?? "",
                            DueDate = Convert.ToDateTime(dueDate),
                            Priority = priority.ToString() ?? ""
                        };
                    }
                    catch
                    {
                        return new UpcomingMaintenanceDto();
                    }
                }).ToList(),
                MaintenanceHistory = new List<object>()
            };
        }
        catch
        {
            return new MaintenanceScheduleDto();
        }
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