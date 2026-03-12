using IOTask.Enums;
using IOTask.Loggers;

namespace IOTask.FileProcessorClass
{
    public class FileProcessor
    {
        private readonly FileInformation _fileInformation;
        private readonly ILogger _logger;
        private readonly IFileActionExecutor _executor;
        private readonly string _basePath;


        public FileProcessor(FileInformation fileInformation, ILogger fileLogger, IFileActionExecutor executor, string basePath)
        {
            _fileInformation = fileInformation;
            _logger = fileLogger;
            _executor = executor;
            _basePath = basePath;
        }

        public async Task RunAsync(int delay)
        {
            if (string.IsNullOrEmpty(_fileInformation.FilePath))
                throw new ArgumentException("FilePath не может быть пустым", nameof(_fileInformation.FilePath));

            if (delay < 0)
                throw new ArgumentException("Delay не может быть отрицательным", nameof(delay));

            string safeSourcePath = ValidatePath(_fileInformation.FilePath);

            string? safeDestinationPath = null;
            if (_fileInformation.DestinationPath != null)
            {
                safeDestinationPath = ValidatePath(_fileInformation.DestinationPath);
            }

            await Task.Delay(delay);

            switch (_fileInformation.Action)
            {
                case FileActions.CREATE:

                    await ExecuteAction(() => _executor.CreateFile(safeSourcePath, _fileInformation.Content));
                    
                    break;

                case FileActions.DELETE:

                    await ExecuteAction(() => _executor.DeleteFile(safeSourcePath));

                    break;

                case FileActions.UPPERCASE:

                    await ExecuteAction(() => _executor.ToUpperCase(safeSourcePath));

                    break;

                case FileActions.LOWERCASE:

                    await ExecuteAction(() => _executor.ToLowerCase(safeSourcePath));

                    break;

                case FileActions.REMOVEDUPS:

                    await ExecuteAction(() => _executor.RemoveDups(safeSourcePath));

                    break;

                case FileActions.COPY:

                    if (string.IsNullOrEmpty(safeDestinationPath))
                    { 
                        throw new InvalidOperationException($"Для действия COPY в {_fileInformation.FilePath} требуется destinationPath");
                    }
                    await ExecuteAction(() => _executor.CopyFile(safeSourcePath, safeDestinationPath));
                    
                    break;


                case FileActions.MOVE:

                    if (string.IsNullOrEmpty(safeDestinationPath))
                    { 
                        throw new InvalidOperationException($"Для действия MOVEв файле {_fileInformation.FilePath} требуется destinationPath");
                    }
                    await ExecuteAction(() => _executor.MoveFile(safeSourcePath, safeDestinationPath));

                    break;

                case FileActions.READ:

                    await ExecuteAction(() => _executor.ReadFile(safeSourcePath));

                    break;

                case FileActions.REPLACE:
                    if (string.IsNullOrEmpty(_fileInformation.OldText))
                        throw new InvalidOperationException($"Для REPLACE требуется oldText в файле {_fileInformation.FilePath}");

                    var actualNewText = _fileInformation.NewText ?? string.Empty;

                    await ExecuteAction(() => _executor.ReplaceText(safeSourcePath, _fileInformation.OldText!, actualNewText));
                    break;

                default:
                    throw new NotImplementedException("Действие не распознано!");

            }

        }

        private async Task ExecuteAction(Func<Task> action)
        {
            try
            {
                await action();
                await _logger.LogAsync(_fileInformation.Action, _fileInformation.FilePath, _fileInformation.Content, _fileInformation.DestinationPath);
            }
            catch (Exception e)
            {
                await _logger.LogAsync(_fileInformation.Action, _fileInformation.FilePath, _fileInformation.Content, _fileInformation.DestinationPath, e.Message);
                throw;
            }
        }

        private string ValidatePath(string inputPath)
        {
            if (string.IsNullOrEmpty(inputPath))
                throw new ArgumentException("Путь не может быть пустым", nameof(inputPath));

            var fullPath = Path.GetFullPath(Path.Combine(_basePath, inputPath));
            var fullBase = Path.GetFullPath(_basePath);

            if (!fullBase.EndsWith(Path.DirectorySeparatorChar) &&
                !fullBase.EndsWith(Path.AltDirectorySeparatorChar))
            {
                fullBase += Path.DirectorySeparatorChar;
            }

            if (!fullPath.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase) &&
                !fullPath.Equals(fullBase.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                                 StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    $"Path '{inputPath}' is outside base directory '{_basePath}'.\n" +
                    $"Full path: {fullPath}\n" +
                    $"Base directory: {fullBase}");
            }

            return fullPath;
        }
    }
}
