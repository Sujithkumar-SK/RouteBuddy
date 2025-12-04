using Kanini.RouteBuddy.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Kanini.RouteBuddy.Data.Infrastructure;

public class AdoDbReader : IDbReader
{
    private readonly string _connectionString;
    private readonly ILogger<AdoDbReader> _logger;

    public AdoDbReader(IConfiguration configuration, ILogger<AdoDbReader> logger)
    {
        _connectionString = configuration.GetConnectionString("DatabaseConnectionString")
                            ?? throw new InvalidOperationException(MagicStrings.ErrorMessages.DatabaseError);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[]? parameters = null)
    {
        var table = new DataTable();

        try
        {
            _logger.LogInformation("Executing stored procedure: {ProcedureName}", procedureName);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            table.Load(reader);

            _logger.LogInformation("Successfully executed stored procedure: {ProcedureName}", procedureName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing stored procedure: {ProcedureName}", procedureName);
            throw;
        }

        return table;
    }

    public async Task<T?> ExecuteScalarAsync<T>(string procedureName, SqlParameter[]? parameters = null)
    {
        try
        {
            _logger.LogInformation("Executing scalar stored procedure: {ProcedureName}", procedureName);

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            if (parameters != null)
                command.Parameters.AddRange(parameters);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return result == null || result == DBNull.Value ? default(T) : (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar stored procedure: {ProcedureName}", procedureName);
            throw;
        }
    }
}