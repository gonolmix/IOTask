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
                string settingsFile = args.Length > 0 ? args[0] : "settings.json";

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

                    if (settings.Files.Count == 0)
                {
                    throw new InvalidOperationException("Массив files пуст.");
                }

                    var logger = new FileLogger();
                    var executor = new FileActionExecutor();

                    foreach (var fileOp in settings.Files)
                    {
                        try
                        {
                            var processor = new FileProcessor(fileOp, logger, executor, settings.BaseDirectory);
                            await processor.RunAsync(settings.Delay);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            Console.WriteLine($"Error with {fileOp.Action} for {fileOp.FilePath}: {ex.Message}");
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
                    return 1;
                }
            }
        }
    }
