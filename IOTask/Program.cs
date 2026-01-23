using IOTask.Data;
using IOTask.Loggers;
using Newtonsoft.Json;
using System.IO;
using IOTask.FileProcessorClass;

namespace IOTask
{
    internal class Program
    {
            static async Task Main(string[] args)
            {
                const string settingsFile = "settings.json";

                if (!File.Exists(settingsFile))
                {
                    Console.WriteLine($"Файл '{settingsFile}' не найден в текущей директории.");
                    Console.WriteLine($"Текущая директория: {Environment.CurrentDirectory}");
                    Console.ReadKey();
                    return;
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
                        string? oldText = null;
                        string? newText = null;

                        if (fileOp.Action == Enums.FileActions.REPLACE)
                        {
                            Console.WriteLine($"\nЗамена текста в файле: {fileOp.FilePath}");
                            Console.Write("Введите текст для замены: ");
                            oldText = Console.ReadLine();
                            Console.Write("Введите новый текст: ");
                            newText = Console.ReadLine();
                        }

                        var processor = new FileProcessor(fileOp, logger, executor);
                        await processor.RunAsync(settings.Delay, oldText, newText);
                    }

                    Console.WriteLine("\nОбработка завершена успешно.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nОшибка: {ex.Message}");
                    Console.WriteLine("Причина: " + ex.ToString());
                }

                Console.ReadKey();
            }
        }
    }
