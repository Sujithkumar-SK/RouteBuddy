namespace Kanini.RouteBuddy.Common.Services;

public static class ScheduleFileLogger
{
    private static readonly string LogsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
    
    public static void LogInfo(string message, params object[] args)
    {
        WriteLog("INFO", message, args);
    }
    
    public static void LogError(string message, Exception? exception = null, params object[] args)
    {
        var logMessage = string.Format(message, args);
        if (exception != null)
            logMessage += Environment.NewLine + exception.ToString();
        
        WriteLog("ERROR", logMessage);
        WriteToErrorFile(logMessage);
    }
    
    public static void LogWarning(string message, params object[] args)
    {
        WriteLog("WARN", message, args);
    }
    
    private static void WriteLog(string level, string message, params object[] args)
    {
        try
        {
            Directory.CreateDirectory(LogsDirectory);
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] SCHEDULE: {string.Format(message, args)}";
            var filePath = Path.Combine(LogsDirectory, $"schedule-{DateTime.Now:yyyy-MM-dd}.txt");
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
        catch { }
    }
    
    private static void WriteToErrorFile(string message)
    {
        try
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] SCHEDULE: {message}";
            var filePath = Path.Combine(LogsDirectory, $"schedule-errors-{DateTime.Now:yyyy-MM-dd}.txt");
            File.AppendAllText(filePath, logEntry + Environment.NewLine);
        }
        catch { }
    }
}