using IOTask.Enums;
using IOTask.Loggers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace IOTask.FileProcessorClass
{
    public class FileProcessor
    {
        public FileInformation _fileInformation;
        public ILogger _logger;
        public IFileActionExecutor _executor;


        public FileProcessor(FileInformation fileInformation, ILogger fileLogger, IFileActionExecutor executor)
        {
            _fileInformation = fileInformation;
            _logger = fileLogger;
            _executor = executor;
        }

        public async Task RunAsync(int delay, string? oldtext = null, string? newtext = null)
        {
            await Task.Delay(delay);
            switch (_fileInformation.Action)
            {
                case FileActions.CREATE:

                    await ExecuteAction(() => _executor.CreateFile(_fileInformation.FilePath));

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

                    if (_fileInformation.Params == null)
                    {
                        throw new InvalidOperationException($"Параметр 'params' обязателен для действия {_fileInformation.Action}");
                    }
                    else
                    {
                        await ExecuteAction(() => _executor.CopyFile(_fileInformation.FilePath, _fileInformation.Params));
                    }

                    break;

                case FileActions.MOVE:

                    if (_fileInformation.Params == null)
                    {
                        throw new InvalidOperationException($"Параметр 'params' обязателен для действия {_fileInformation.Action}");
                    }
                    else
                    {
                        await ExecuteAction(() => _executor.MoveFile(_fileInformation.FilePath, _fileInformation.Params));
                    }

                    break;

                case FileActions.READ:

                    await ExecuteAction(() => _executor.ReadFile(_fileInformation.FilePath));

                    break;

                case FileActions.REPLACE:

                    if (newtext == null || oldtext == null)
                    {
                        throw new InvalidOperationException($"Отсутствует текст для замены в файле {_fileInformation.FilePath}!");
                    }
                    else
                    {
                        await ExecuteAction(() => _executor.ReplaceText(_fileInformation.FilePath, oldtext, newtext));
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
                await _logger.LogAsync(_fileInformation.Action, _fileInformation.FilePath, _fileInformation.Params);
            }
            catch (Exception e)
            {
                await _logger.LogAsync(_fileInformation.Action, _fileInformation.FilePath, _fileInformation.Params, e.Message);
            }
        }
    }
}
