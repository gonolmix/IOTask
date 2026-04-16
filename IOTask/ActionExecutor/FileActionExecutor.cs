using System.Text;
using System.Text.RegularExpressions;

namespace IOTask
{
    public class FileActionExecutor : IFileActionExecutor
    {
        public async Task CreateFile(string filePath, string? content = null)
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(filePath, content ?? string.Empty);
        }

        public Task DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");
                
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        public async Task ToUpperCase(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var text = await File.ReadAllTextAsync(filePath);
            await File.WriteAllTextAsync(filePath, text.ToUpperInvariant());
        }

        public async Task ToLowerCase(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var text = await File.ReadAllTextAsync(filePath);
            await File.WriteAllTextAsync(filePath, text.ToLowerInvariant());
        }

        /// Удаляет повторяющиеся слова, сохраняя структуру текста.
        /// Подходит для небольших файлов (<100MB).
        /// Поддерживает Unicode 
        public async Task RemoveDups(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            const long MaxFileSizeBytes = 100 * 1024 * 1024;

            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > MaxFileSizeBytes)
                throw new InvalidOperationException($"Файл {filePath} слишком большой для обработки (>100 МБ)");

            var text = await File.ReadAllTextAsync(filePath);
            var matches = Regex.Matches(text, @"\b\w+\b");
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new StringBuilder();
            int lastIndex = 0;

            foreach (Match match in matches)
            {
                result.Append(text.Substring(lastIndex, match.Index - lastIndex));
                if (seen.Add(match.Value))
                    result.Append(match.Value);
                lastIndex = match.Index + match.Length;
            }
            result.Append(text.Substring(lastIndex));

            await File.WriteAllTextAsync(filePath, result.ToString());
        }

        public Task CopyFile(string sourcePath, string destinationPath)
        {
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            File.Copy(sourcePath, destinationPath, overwrite: true);
            return Task.CompletedTask;
        }

        public Task MoveFile(string sourcePath, string destinationPath)
        {
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            File.Move(sourcePath, destinationPath, overwrite: true);
            return Task.CompletedTask;
        }

        public async Task ReadFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            var content = await File.ReadAllTextAsync(filePath);
            Console.WriteLine(content);
        }

        public async Task ReplaceText(string filePath, string oldText, string newText)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не найден: {filePath}");

            if (string.IsNullOrEmpty(oldText))
            {
                throw new ArgumentException("oldText не может быть пустым", nameof(oldText));
            }

            var content = await File.ReadAllTextAsync(filePath);
            var updatedContent = content.Replace(oldText, newText);
            await File.WriteAllTextAsync(filePath, updatedContent);
        }

    }
}
