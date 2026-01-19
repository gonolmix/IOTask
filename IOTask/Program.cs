using Newtonsoft.Json;
using System.IO;
using System.Text.Json.Serialization;

namespace IOTask
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = null;
            while (!File.Exists(filePath))
            {
                Console.WriteLine("Введите путь к файлу:");
                filePath = Console.ReadLine();
                if (!File.Exists(filePath))
                    Console.WriteLine("Некорректный путь к файлу!");
                else
                {
                    try
                    {
                        var jsonContent = File.ReadAllText(filePath);

                        var jsonSettings = JsonConvert.DeserializeObject<JsonSettings>(jsonContent);

                        foreach (var file in jsonSettings.Files)
                        {
                            file.Run(jsonSettings.Delay);
                        }
                    }
                    catch (Exception e)
                    { 
                        Console.WriteLine($"Не удалось обработать файл {filePath}: {e.Message}");
                    }
                }
            }

            Console.ReadKey();



        }
    }
}
