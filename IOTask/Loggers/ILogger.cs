using IOTask.Enums;

namespace IOTask.Loggers
{
    public interface ILogger
    {
        Task LogAsync(FileActions action, string filePath, string? content = null, string? destinationPath = null, string? errorMessage = null);
    }
}
