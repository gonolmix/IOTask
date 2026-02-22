using IOTask.Data;
using IOTask.Loggers;
using Newtonsoft.Json;
using IOTask.FileProcessorClass;

namespace IOTask
{
    internal class Program
    {
            static async Task<int> Main(string[] args)
            {
                const string settingsFile = "settings.json";
                const string oldText = "old";
                const string newText = "new";

                int errorCount = 0;

                if (!File.Exists(settingsFile))
                {
                    Console.WriteLine($"Файл '{settingsFile}' не найден в текущей директории.");
                    Console.WriteLine($"Текущая директория: {Environment.CurrentDirectory}");
                    return 1;
                }

                try
                {
                    string jsonContent = await File.ReadAllTextAsync(settingsFile);
                    if (string.IsNullOrWhiteSpace(jsonContent))
                        throw new InvalidDataException("Файл settings.json пуст.");

                    var settings = JsonConvert.DeserializeObject<JsonSettings>(jsonContent)
                        ?? throw new InvalidOperationException("Не удалось десериализовать settings.json.");

                    if (settings.Files == null)
                        throw new InvalidOperationException("Раздел 'files' отсутствует в settings.json.");

                    var logger = new FileLogger();
                    var executor = new FileActionExecutor();

                    foreach (var fileOp in settings.Files)
                    {
                    //string? oldText = null;
                    //string? newText = null;

                    //if (fileOp.Action == Enums.FileActions.REPLACE)
                    //{
                    //    Console.WriteLine($"\nЗамена текста в файле: {fileOp.FilePath}");
                    //    Console.Write("Введите текст для замены: ");
                    //    oldText = Console.ReadLine();
                    //    Console.Write("Введите новый текст: ");
                    //    newText = Console.ReadLine();
                    //}
                        try
                        {
                            var processor = new FileProcessor(fileOp, logger, executor);
                            await processor.RunAsync(settings.Delay, oldText, newText);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                        }
                    }
                    if (errorCount > 0)
                    {
                        Console.WriteLine($"\n Завершено с ошибками: {errorCount}");
                        return 1;
                    }

                    Console.WriteLine("\nОбработка завершена успешно.");
                    return 0;

            }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nОшибка: {ex.Message}");
                    Console.WriteLine("Причина: " + ex.ToString());
                    return 1;
                }
            }
        }
    }
