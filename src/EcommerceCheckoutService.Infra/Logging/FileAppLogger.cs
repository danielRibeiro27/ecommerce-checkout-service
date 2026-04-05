namespace EcommerceCheckoutService.Infra.Logging;

public class FileAppLogger : IAppLogger
{
    private readonly string _filePath;
    private readonly object _lock = new();

    public FileAppLogger(string filePath)
    {
        _filePath = filePath;
        var dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
    }

    public void Info(string message) => Write("INFO", message);

    public void Warning(string message) => Write("WARN", message);

    public void Error(string message, Exception? exception = null)
    {
        var fullMessage = exception is not null
            ? $"{message} | Exception: {exception}"
            : message;
        Write("ERROR", fullMessage);
    }

    private void Write(string level, string message)
    {
        var line = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] [{level}] {message}{Environment.NewLine}";
        lock (_lock)
        {
            File.AppendAllText(_filePath, line);
        }
    }
}
