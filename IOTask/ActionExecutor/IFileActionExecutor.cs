namespace IOTask
{
    public interface IFileActionExecutor
    {
        Task CreateFile(string filePath, string? content = null);

        Task DeleteFile(string filePath);

        Task ToUpperCase(string filePath);

        Task ToLowerCase(string filePath);

        Task RemoveDups(string filePath);

        Task CopyFile(string sourcePath, string destinationPath);

        Task MoveFile(string sourcePath, string destinationPath);

        Task ReadFile(string filePath);

        Task ReplaceText(string filePath, string oldText, string newText);
    }
}
