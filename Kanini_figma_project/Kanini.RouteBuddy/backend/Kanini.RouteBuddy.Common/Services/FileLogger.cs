using Microsoft.Extensions.Logging;

namespace Kanini.RouteBuddy.Common.Services;

public class FileLogger : ILogger
{
    private readonly string _filePath;
    private readonly string _categoryName;

    public FileLogger(string categoryName, string filePath)
    {
        _categoryName = categoryName;
        _filePath = filePath;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {_categoryName}: {formatter(state, exception)}";
        if (exception != null)
            logEntry += Environment.NewLine + exception.ToString();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.AppendAllText(_filePath, logEntry + Environment.NewLine);
        }
        catch
        {
            // Ignore file write errors to prevent logging loops
        }
    }
}