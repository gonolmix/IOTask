using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IOTask
{
    public class FileProcessor
    {
        [JsonProperty("path")]
        public string FilePath { get; set; }
        [JsonProperty("action")]
        public FileActions Action { get; set; }
        [JsonProperty("params")]
        public string? Params { get; set; }

        public FileProcessor() { }

        public FileProcessor(string filePath, FileActions action, string? @params)
        {
            FilePath = filePath;
            Action = action;
            Params = @params;
        }

        public void Run(int delay)
        {
            Thread.Sleep(delay);
            switch (Action)
            {
                case FileActions.CREATE:

                    try
                    {
                        File.WriteAllText(FilePath, Params);
                        this.Log();
                        Console.WriteLine($"Файл {FilePath} создан");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось создать файл {FilePath}: {e.Message}"); 
                    }

                    break;

                case FileActions.DELETE:

                    try
                    {
                        File.Delete(FilePath);
                        this.Log();
                        Console.WriteLine($"Файл {FilePath} удалён");
                    }
                    catch (Exception e)
                    {
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось удалить файл {FilePath}: {e.Message}");
                    }
                    break;

                case FileActions.UPPERCASE:

                    try
                    {
                        string upperText = File.ReadAllText(FilePath).ToUpper();
                        File.WriteAllText(FilePath, upperText);
                        Console.WriteLine($"Текст файла {FilePath} переведён в верхний регистр");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось перевести текст файла {FilePath} в верхний регистр: {e.Message}");
                    }
                    break;

                case FileActions.LOWERCASE:

                    try
                    {
                        string lowerText = File.ReadAllText(FilePath).ToLower();
                        File.WriteAllText(FilePath, lowerText);
                        Console.WriteLine($"Текст файла {FilePath} переведён в нижний регистр");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось перевести текст файла {FilePath} в нижний регистр: {e.Message}");
                    }


                    break;

                case FileActions.REMOVEDUPS:
                    try
                    {
                        char[] separators = { ' ', '\t', '\n', '\r', '.', ',', '!', '?', ';', ':', '-', '(', ')', '[', ']', '"' };
                        string fileTextToSplit = File.ReadAllText(FilePath);

                        // Версия без учёта регистра (упрощённая, т.к. в файл будет перезаписываться значения в нижнем регистре)
                        // string fileTextToSplit = File.ReadAllText(_filePath).ToLower();

                        var splittedText = fileTextToSplit.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        var uniqWords = splittedText.Distinct().ToList();
                        File.WriteAllLines(FilePath, uniqWords);
                        Console.WriteLine($"Повторяющиеся слова в файле {FilePath} удалены!");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось удалить повторы слов в файле {FilePath}: {e.Message}");
                    }

                    break;

                case FileActions.COPY:

                    try 
                    {
                        File.Copy(FilePath, Params);
                        Console.WriteLine($"Файл {FilePath} скопирован в директорию {Params}");
                    }
                    catch (Exception e)
                    {
                        this.Log(e.Message);
                        Console.WriteLine($"Не удлось скопировать файл {FilePath} в директорию {Params}: {e.Message}");
                    }


                    break;

                case FileActions.MOVE:

                    try
                    {
                        File.Move(FilePath, Params);
                        Console.WriteLine($"Файл {FilePath} перемещён в директорию {Params}");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось переместить файл {FilePath} в директорию {Params}: {e.Message}");
                    }

                    break;

                case FileActions.READ:

                    try
                    {
                        Console.WriteLine($"Содержимое файла {FilePath}\n");
                        Console.WriteLine(File.ReadAllText(FilePath));
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось прочесть содержимое файла {FilePath}: {e.Message}");
                    }


                    break;

                case FileActions.REPLACE:

                    try 
                    {
                        Console.WriteLine($"Введите текст, который хотите заменить в файле {FilePath}:");
                        var oldText = Console.ReadLine();
                        Console.WriteLine($"Введите новый текст:");
                        var newText = Console.ReadLine();
                        
                        var fileContent = File.ReadAllText(FilePath);

                        var textForReplace = fileContent.Replace(oldText, newText);

                        File.WriteAllText(FilePath, textForReplace);
                        Console.WriteLine("Если введённый текст был в файле, то он успешно заменён!");
                    }
                    catch (Exception e)
                    { 
                        this.Log(e.Message);
                        Console.WriteLine($"Не удалось заменить текст в файле {FilePath}: {e.Message}");
                    }


                    break;

                default:
                    throw new NotImplementedException("Действие не распознано!");

            }

        }
        public void Log()
        {
            switch (Action)
            {
                case FileActions.CREATE:

                    var createMassage = $"{DateTime.Now}: Файл {FilePath} создан;\n";
                    File.AppendAllText("log.txt", createMassage);

                    break;

                case FileActions.DELETE:

                    var deleteMassage = $"{DateTime.Now}: Файл {FilePath} удалён;\n";
                    File.AppendAllText("log.txt", deleteMassage);

                    break;

                case FileActions.UPPERCASE:

                    var upperCaseMassage = $"{DateTime.Now}: Текст в файле {FilePath} приведён в верхний регистр;\n";
                    File.AppendAllText("log.txt", upperCaseMassage);

                    break;

                case FileActions.LOWERCASE:

                    var lowerCaseMassage = $"{DateTime.Now}: Текст в файле {FilePath} приведён в нижний регистр;\n";
                    File.AppendAllText("log.txt", lowerCaseMassage);

                    break;

                case FileActions.REMOVEDUPS:

                    var removeDupsMassage = $"{DateTime.Now}: Повторы слов в файле {FilePath} удалены;\n";
                    File.AppendAllText("log.txt", removeDupsMassage);

                    break;

                case FileActions.COPY:

                    var copyMassage = $"{DateTime.Now}: Файл {FilePath} скопирован в директорию {Params};\n";
                    File.AppendAllText("log.txt", copyMassage);

                    break;

                case FileActions.MOVE:

                    var moveMassage = $"{DateTime.Now}: Файл {FilePath} перемещён в директорию {Params};\n";
                    File.AppendAllText("log.txt", moveMassage);

                    break;

                case FileActions.READ:

                    var readMassage = $"{DateTime.Now}: Содержимое файла {FilePath} выведено в консоли;\n";
                    File.AppendAllText("log.txt", readMassage);

                    break;

                case FileActions.REPLACE:
                    
                    var replaceMassage = $"{DateTime.Now}: Содержимое файла {FilePath} заменено пользователем;\n";
                    File.AppendAllText("log.txt", replaceMassage);

                    break;

                default:

                    throw new NotImplementedException("Действие не распознано!");

            }
        }
        public void Log(string exMassage)
        {
            var exeptionMassage = $"{DateTime.Now}: Не удалось выполнить действие {Action} для файла {FilePath}. Причина: {exMassage};\n";
            File.AppendAllText("log.txt", exeptionMassage);
        }
    }
}
