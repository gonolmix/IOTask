using IOTask.Enums;
using IOTask.Loggers;

namespace IOTask.FileProcessorClass
{
    public class FileProcessor
    {
        private readonly FileInformation _fileInformation;
        private readonly ILogger _logger;
        private readonly IFileActionExecutor _executor;


        public FileProcessor(FileInformation fileInformation, ILogger fileLogger, IFileActionExecutor executor)
        {
            _fileInformation = fileInformation;
            _logger = fileLogger;
            _executor = executor;
        }

        public async Task RunAsync(int delay, string? oldText = null, string? newText = null)
        {
            if (string.IsNullOrEmpty(_fileInformation.FilePath))
                throw new ArgumentException("FilePath не может быть пустым", nameof(_fileInformation.FilePath));

            if (delay < 0)
                throw new ArgumentException("Delay не может быть отрицательным", nameof(delay));

            await Task.Delay(delay);

            switch (_fileInformation.Action)
            {
                case FileActions.CREATE:

                    await ExecuteAction(() => _executor.CreateFile(_fileInformation.FilePath, _fileInformation.Content));
                    
                    break;

                case FileActions.DELETE:

                    await ExecuteAction(() => _executor.DeleteFile(_fileInformation.FilePath));

                    break;

                case FileActions.UPPERCASE:

                    await ExecuteAction(() => _executor.ToUpperCase(_fileInformation.FilePath));

                    break;

                case FileActions.LOWERCASE:

                    await ExecuteAction(() => _executor.ToLowerCase(_fileInformation.FilePath));

                    break;

                case FileActions.REMOVEDUPS:

                    await ExecuteAction(() => _executor.RemoveDups(_fileInformation.FilePath));

                    break;

                case FileActions.COPY:

                    if (string.IsNullOrEmpty(_fileInformation.DestinationPath))
                    { 
                        throw new InvalidOperationException($"Для действия COPY в {_fileInformation.FilePath} требуется destinationPath");
                    }
                    await ExecuteAction(() => _executor.CopyFile(_fileInformation.FilePath, _fileInformation.DestinationPath));
                    
                    break;


                case FileActions.MOVE:

                    if (string.IsNullOrEmpty(_fileInformation.DestinationPath))
                    { 
                        throw new InvalidOperationException($"Для действия MOVEв файле {_fileInformation.FilePath} требуется destinationPath");
                    }
                    await ExecuteAction(() => _executor.MoveFile(_fileInformation.FilePath, _fileInformation.DestinationPath));

                    break;

                case FileActions.READ:

                    await ExecuteAction(() => _executor.ReadFile(_fileInformation.FilePath));

                    break;

                case FileActions.REPLACE:

                    if (newText == null || oldText == null)
                    {
                        throw new InvalidOperationException($"Отсутствует текст для замены в файле {_fileInformation.FilePath}!");
                    }
                    else
                    {
                        await ExecuteAction(() => _executor.ReplaceText(_fileInformation.FilePath, oldText, newText));
                    }

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
            }
        }
    }
}
