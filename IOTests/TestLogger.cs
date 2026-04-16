using IOTask.Enums;
using IOTask.Loggers;
using System.Text;

namespace IOTests
{
    public class TestLogger : ILogger
    {
        private readonly StringBuilder _log = new();

        public string LogContent => _log.ToString();

        public Task LogAsync(FileActions action, string filePath, string? content = null, string? destinationPath = null, string? errorMessage = null)
        {
            var msg = errorMessage != null
                ? $"ERROR [{action}] {filePath}: {errorMessage}"
                : $"SUCCESS [{action}] {filePath}";
            _log.AppendLine(msg);
            return Task.CompletedTask;
        }
    }
}
