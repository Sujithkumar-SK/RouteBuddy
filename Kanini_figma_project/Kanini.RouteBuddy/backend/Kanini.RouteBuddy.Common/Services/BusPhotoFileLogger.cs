namespace Kanini.RouteBuddy.Common.Services;

public static class BusPhotoFileLogger
{
    private static readonly string LogFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "BusPhoto.log");

    static BusPhotoFileLogger()
    {
        var logDirectory = Path.GetDirectoryName(LogFilePath);
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory!);
        }
    }

    public static void LogInfo(string message, params object[] args)
    {
        var logMessage = $"[INFO] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}";
        File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
    }

    public static void LogWarning(string message, params object[] args)
    {
        var logMessage = $"[WARNING] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {string.Format(message, args)}";
        File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
    }

    public static void LogError(string message, Exception? ex = null)
    {
        var logMessage = $"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}";
        if (ex != null)
        {
            logMessage += $" | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
        }
        File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
    }
}