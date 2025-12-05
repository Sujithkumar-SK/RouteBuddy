using Microsoft.Data.SqlClient;
using System.Data;

namespace Kanini.RouteBuddy.Data.Infrastructure;

public interface IDbReader
{
    Task<DataTable> ExecuteStoredProcedureAsync(string procedureName, SqlParameter[]? parameters = null);
    Task<T?> ExecuteScalarAsync<T>(string procedureName, SqlParameter[]? parameters = null);
}