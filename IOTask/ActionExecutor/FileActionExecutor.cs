using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        public async Task DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
               await Task.Run(() => File.Delete(filePath));
        }

        public async Task ToUpperCase(string filePath)
        {
            var text = await File.ReadAllTextAsync(filePath);
            await File.WriteAllTextAsync(filePath, text.ToUpper());
        }

        public async Task ToLowerCase(string filePath)
        {
            var text = await File.ReadAllTextAsync(filePath);
            await File.WriteAllTextAsync(filePath, text.ToLower());
        }

        public async Task RemoveDups(string filePath)
        {
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

        public async Task CopyFile(string sourcePath, string destinationPath)
        {
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            await Task.Run(() => File.Copy(sourcePath, destinationPath, overwrite: true));
        }

        public async Task MoveFile(string sourcePath, string destinationPath)
        {
            var destDir = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(destDir))
                Directory.CreateDirectory(destDir);

            await Task.Run(() => File.Move(sourcePath, destinationPath, overwrite: true));
        }

        public async Task ReadFile(string filePath)
        {
            var content = await File.ReadAllTextAsync(filePath);
            Console.WriteLine(content);
        }

        public async Task ReplaceText(string filePath, string oldText, string newText)
        {
            if (string.IsNullOrEmpty(oldText))
            {
                return;
            }

            var content = await File.ReadAllTextAsync(filePath);
            var updatedContent = content.Replace(oldText, newText);
            await File.WriteAllTextAsync(filePath, updatedContent);
        }

    }
}
