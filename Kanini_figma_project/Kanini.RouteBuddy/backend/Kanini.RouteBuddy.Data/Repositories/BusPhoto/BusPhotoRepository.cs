using Microsoft.EntityFrameworkCore;
using Kanini.RouteBuddy.Data.DatabaseContext;
using Kanini.RouteBuddy.Common.Utility;
using Kanini.RouteBuddy.Common.Errors;
using Kanini.RouteBuddy.Common.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Kanini.RouteBuddy.Data.Repositories.BusPhoto;

public class BusPhotoRepository : IBusPhotoRepository
{
    private readonly RouteBuddyDatabaseContext _context;
    private readonly string _connectionString;

    public BusPhotoRepository(RouteBuddyDatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")!;
    }

    public async Task<Result<Domain.Entities.BusPhoto>> CreateAsync(Domain.Entities.BusPhoto busPhoto)
    {
        try
        {
            BusPhotoFileLogger.LogInfo("Creating bus photo for bus ID: {0}", busPhoto.BusId);
            
            _context.BusPhotos.Add(busPhoto);
            await _context.SaveChangesAsync();
            
            BusPhotoFileLogger.LogInfo("Bus photo created successfully with ID: {0}", busPhoto.BusPhotoId);
            return Result.Success(busPhoto);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error creating bus photo", ex);
            return Result.Failure<Domain.Entities.BusPhoto>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoCreateFailed, BusPhotoMessages.ErrorMessages.BusPhotoCreateFailed));
        }
    }

    public async Task<Result<Domain.Entities.BusPhoto>> GetByIdAsync(int busPhotoId)
    {
        try
        {
            BusPhotoFileLogger.LogInfo("Getting bus photo by ID: {0}", busPhotoId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusPhotoById", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusPhotoId", busPhotoId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            if (await reader.ReadAsync())
            {
                var busPhoto = new Domain.Entities.BusPhoto
                {
                    BusPhotoId = reader.GetInt32("BusPhotoId"),
                    BusId = reader.GetInt32("BusId"),
                    ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                    Caption = reader.IsDBNull("Caption") ? null : reader.GetString("Caption"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn") ? null : reader.GetDateTime("UpdatedOn")
                };
                
                BusPhotoFileLogger.LogInfo("Bus photo retrieved successfully: {0}", busPhotoId);
                return Result.Success(busPhoto);
            }
            
            BusPhotoFileLogger.LogWarning("Bus photo not found: {0}", busPhotoId);
            return Result.Failure<Domain.Entities.BusPhoto>(Error.NotFound(BusPhotoMessages.ErrorCodes.BusPhotoNotFound, BusPhotoMessages.ErrorMessages.BusPhotoNotFound));
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error retrieving bus photo", ex);
            return Result.Failure<Domain.Entities.BusPhoto>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoGetFailed, BusPhotoMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<IEnumerable<Domain.Entities.BusPhoto>>> GetByBusIdAsync(int busId)
    {
        try
        {
            BusPhotoFileLogger.LogInfo("Getting bus photos by bus ID: {0}", busId);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusPhotosByBusId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);
            
            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            
            var busPhotos = new List<Domain.Entities.BusPhoto>();
            while (await reader.ReadAsync())
            {
                busPhotos.Add(new Domain.Entities.BusPhoto
                {
                    BusPhotoId = reader.GetInt32("BusPhotoId"),
                    BusId = reader.GetInt32("BusId"),
                    ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                    Caption = reader.IsDBNull("Caption") ? null : reader.GetString("Caption"),
                    CreatedBy = reader.GetString("CreatedBy"),
                    CreatedOn = reader.GetDateTime("CreatedOn"),
                    UpdatedBy = reader.IsDBNull("UpdatedBy") ? null : reader.GetString("UpdatedBy"),
                    UpdatedOn = reader.IsDBNull("UpdatedOn") ? null : reader.GetDateTime("UpdatedOn")
                });
            }
            
            BusPhotoFileLogger.LogInfo("Retrieved {0} bus photos for bus ID: {1}", busPhotos.Count, busId);
            return Result.Success<IEnumerable<Domain.Entities.BusPhoto>>(busPhotos);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error getting bus photos by bus ID", ex);
            return Result.Failure<IEnumerable<Domain.Entities.BusPhoto>>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoGetByBusFailed, BusPhotoMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<Domain.Entities.BusPhoto>> UpdateAsync(Domain.Entities.BusPhoto busPhoto)
    {
        try
        {
            BusPhotoFileLogger.LogInfo("Updating bus photo: {0}", busPhoto.BusPhotoId);
            
            _context.BusPhotos.Attach(busPhoto);
            _context.Entry(busPhoto).Property(x => x.Caption).IsModified = true;
            _context.Entry(busPhoto).Property(x => x.UpdatedBy).IsModified = true;
            _context.Entry(busPhoto).Property(x => x.UpdatedOn).IsModified = true;
            await _context.SaveChangesAsync();
            
            BusPhotoFileLogger.LogInfo("Bus photo updated successfully: {0}", busPhoto.BusPhotoId);
            return Result.Success(busPhoto);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error updating bus photo", ex);
            return Result.Failure<Domain.Entities.BusPhoto>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoUpdateFailed, BusPhotoMessages.ErrorMessages.BusPhotoUpdateFailed));
        }
    }

    public async Task<Result<bool>> DeleteAsync(int busPhotoId)
    {
        try
        {
            BusPhotoFileLogger.LogInfo("Deleting bus photo: {0}", busPhotoId);
            
            var busPhoto = await _context.BusPhotos.FindAsync(busPhotoId);
            if (busPhoto == null)
            {
                BusPhotoFileLogger.LogWarning("Bus photo not found for deletion: {0}", busPhotoId);
                return Result.Failure<bool>(Error.NotFound(BusPhotoMessages.ErrorCodes.BusPhotoNotFound, BusPhotoMessages.ErrorMessages.BusPhotoNotFound));
            }

            _context.BusPhotos.Remove(busPhoto);
            await _context.SaveChangesAsync();
            
            BusPhotoFileLogger.LogInfo("Bus photo deleted successfully: {0}", busPhotoId);
            return Result.Success(true);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error deleting bus photo", ex);
            return Result.Failure<bool>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoDeleteFailed, BusPhotoMessages.ErrorMessages.BusPhotoDeleteFailed));
        }
    }

    public async Task<Result<bool>> ExistsAsync(int busPhotoId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_CheckBusPhotoExists", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusPhotoId", busPhotoId);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            return Result.Success(count > 0);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error checking bus photo exists", ex);
            return Result.Failure<bool>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoExistsFailed, BusPhotoMessages.ErrorMessages.DatabaseError));
        }
    }

    public async Task<Result<int>> GetCountByBusIdAsync(int busId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand("sp_GetBusPhotoCountByBusId", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@BusId", busId);
            
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            var count = result != null ? (int)result : 0;
            return Result.Success(count);
        }
        catch (Exception ex)
        {
            BusPhotoFileLogger.LogError("Error getting bus photo count", ex);
            return Result.Failure<int>(
                Error.Failure(BusPhotoMessages.ErrorCodes.BusPhotoCountFailed, BusPhotoMessages.ErrorMessages.DatabaseError));
        }
    }
}