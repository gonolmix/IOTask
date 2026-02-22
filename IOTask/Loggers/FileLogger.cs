using IOTask.Enums;

namespace IOTask.Loggers
{
    public class FileLogger : ILogger
    {
        private const string LogFileName = "log.txt";

        public async Task LogAsync(FileActions action, string filePath, string? content = null, string? destinationPath = null, string? errorMessage = null)
        {
            try
            {
                string message = FormatMessage(action, filePath, content, destinationPath, errorMessage);
                await File.AppendAllTextAsync(LogFileName, message + Environment.NewLine);
                Console.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private static string FormatMessage(FileActions action, string filePath, string? content, string? destinationPath, string? errorMessage)
        {
            if (errorMessage != null)
                return $"{DateTime.Now}: Не удалось выполнить действие {action} для файла {filePath}. Причина: {errorMessage};";

            return action switch
            {
                FileActions.CREATE => $"{DateTime.Now}: Файл {filePath} создан;",
                FileActions.DELETE => $"{DateTime.Now}: Файл {filePath} удалён;",
                FileActions.UPPERCASE => $"{DateTime.Now}: Текст в файле {filePath} приведён в верхний регистр;",
                FileActions.LOWERCASE => $"{DateTime.Now}: Текст в файле {filePath} приведён в нижний регистр;",
                FileActions.REMOVEDUPS => $"{DateTime.Now}: Повторы слов в файле {filePath} удалены;",
                FileActions.COPY => $"{DateTime.Now}: Файл {filePath} скопирован в директорию {destinationPath};",
                FileActions.MOVE => $"{DateTime.Now}: Файл {filePath} перемещён в директорию {destinationPath};",
                FileActions.READ => $"{DateTime.Now}: Содержимое файла {filePath} выведено в консоли;",
                FileActions.REPLACE => $"{DateTime.Now}: Содержимое файла {filePath} заменено пользователем;",
                _ => $"Неизвестное действие: {action}"
            };
        }


    }
}
