using IOTask.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTask.Loggers
{
    public class FileLogger : ILogger
    {
        private const string LogFileName = "log.txt";

        public async Task LogAsync(FileActions action, string filePath, string? @params = null, string? errorMessage = null)
        {
            if (errorMessage == null)
            {
                switch (action)
                {
                    case FileActions.CREATE:

                        var createMessage = $"{DateTime.Now}: Файл {filePath} создан;\n";
                        await File.AppendAllTextAsync(LogFileName, createMessage);
                        Console.WriteLine(createMessage);

                        break;

                    case FileActions.DELETE:

                        var deleteMessage = $"{DateTime.Now}: Файл {filePath} удалён;\n";
                        await File.AppendAllTextAsync(LogFileName, deleteMessage);
                        Console.WriteLine(deleteMessage);

                        break;

                    case FileActions.UPPERCASE:

                        var upperCaseMessage = $"{DateTime.Now}: Текст в файле {filePath} приведён в верхний регистр;\n";
                        await File.AppendAllTextAsync(LogFileName, upperCaseMessage);
                        Console.WriteLine(upperCaseMessage);

                        break;

                    case FileActions.LOWERCASE:

                        var lowerCaseMessage = $"{DateTime.Now}: Текст в файле {filePath} приведён в нижний регистр;\n";
                        await File.AppendAllTextAsync(LogFileName, lowerCaseMessage);
                        Console.WriteLine(lowerCaseMessage);

                        break;

                    case FileActions.REMOVEDUPS:

                        var removeDupsMessage = $"{DateTime.Now}: Повторы слов в файле {filePath} удалены;\n";
                        await File.AppendAllTextAsync(LogFileName, removeDupsMessage);
                        Console.WriteLine(removeDupsMessage);

                        break;

                    case FileActions.COPY:

                        var copyMessage = $"{DateTime.Now}: Файл {filePath} скопирован в директорию {@params};\n";
                        await File.AppendAllTextAsync(LogFileName, copyMessage);
                        Console.WriteLine(copyMessage);

                        break;

                    case FileActions.MOVE:

                        var moveMessage = $"{DateTime.Now}: Файл {filePath} перемещён в директорию {@params};\n";
                        await File.AppendAllTextAsync(LogFileName, moveMessage);
                        Console.WriteLine(moveMessage);

                        break;

                    case FileActions.READ:

                        var readMessage = $"{DateTime.Now}: Содержимое файла {filePath} выведено в консоли;\n";
                        await File.AppendAllTextAsync(LogFileName, readMessage);
                        Console.WriteLine(readMessage);

                        break;

                    case FileActions.REPLACE:

                        var replaceMessage = $"{DateTime.Now}: Содержимое файла {filePath} заменено пользователем;\n";
                        await File.AppendAllTextAsync(LogFileName, replaceMessage);
                        Console.WriteLine(replaceMessage);

                        break;

                    default:

                        throw new NotImplementedException("Действие не распознано!");

                }
            }
            else
            {
                var exceptionMessage = $"{DateTime.Now}: Не удалось выполнить действие {action} для файла {filePath}. Причина: {errorMessage};\n";
                await File.AppendAllTextAsync(LogFileName, exceptionMessage);
                Console.WriteLine(exceptionMessage);
            }
        }
    }
}
