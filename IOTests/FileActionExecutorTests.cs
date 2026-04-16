using IOTask;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace IOTests
{
    public class FileActionExecutorTests : IAsyncLifetime
    {
        private readonly string _testDir;
        private readonly FileActionExecutor _executor;

        public FileActionExecutorTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);
            _executor = new FileActionExecutor();
        }

        public async Task DisposeAsync()
        {
            if (Directory.Exists(_testDir))
                Directory.Delete(_testDir, recursive: true);
            await Task.CompletedTask;
        }

        public Task InitializeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("test.txt", "Hello World")]
        [InlineData("subdir/file.txt", "Content with subdir")]
        [InlineData("empty.txt", "")]
        public async Task CreateFile_ShouldCreateFileWithContent(string relativePath, string content)
        {
            var filePath = Path.Combine(_testDir, relativePath);

            await _executor.CreateFile(filePath, content);

            Assert.True(File.Exists(filePath));
            var actual = await File.ReadAllTextAsync(filePath);
            Assert.Equal(content, actual);
        }

        [Theory]
        [InlineData("existing.txt")]
        [InlineData("subdir/nested.txt")]
        public async Task DeleteFile_ShouldDeleteExistingFile(string relativePath)
        {
            var filePath = Path.Combine(_testDir, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            await File.WriteAllTextAsync(filePath, "to delete");

            await _executor.DeleteFile(filePath);

            Assert.False(File.Exists(filePath));
        }

        [Theory]
        [InlineData("nonexistent.txt")]
        public async Task DeleteFile_ShouldThrowWhenFileNotFound(string relativePath)
        {
            var filePath = Path.Combine(_testDir, relativePath);

            await Assert.ThrowsAsync<FileNotFoundException>(() => _executor.DeleteFile(filePath));
        }

        [Theory]
        [InlineData("text.txt", "hello world", "HELLO WORLD")]
        [InlineData("mixed.txt", "Hello WORLD 123", "HELLO WORLD 123")]
        public async Task ToUpperCase_ShouldConvertToInvariantUpper(string fileName, string input, string expected)
        {
            var filePath = Path.Combine(_testDir, fileName);
            await File.WriteAllTextAsync(filePath, input);

            await _executor.ToUpperCase(filePath);
            var result = await File.ReadAllTextAsync(filePath);

            Assert.Equal(expected, result.ToUpperInvariant());
        }

        [Theory]
        [InlineData("text.txt", "HELLO WORLD", "hello world")]
        [InlineData("mixed.txt", "Hello WORLD 123", "hello world 123")]
        public async Task ToLowerCase_ShouldConvertToInvariantLower(string fileName, string input, string expected)
        {
            var filePath = Path.Combine(_testDir, fileName);
            await File.WriteAllTextAsync(filePath, input);

            await _executor.ToLowerCase(filePath);
            var result = await File.ReadAllTextAsync(filePath);

            Assert.Equal(expected, result.ToLowerInvariant());
        }

        [Theory]
        [InlineData("dups.txt", "hello world hello", "hello world ")] 
        [InlineData("case.txt", "Hello hello HELLO", "Hello ")]      
        public async Task RemoveDups_ShouldRemoveDuplicateWords(string fileName, string input, string expectedPrefix)
        {
            var filePath = Path.Combine(_testDir, fileName);
            await File.WriteAllTextAsync(filePath, input);

            await _executor.RemoveDups(filePath);
            var result = await File.ReadAllTextAsync(filePath);

            Assert.StartsWith(expectedPrefix, result, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("source.txt", "dest.txt", "copy content")]
        [InlineData("sub/source.txt", "backup/dest.txt", "nested copy")]
        public async Task CopyFile_ShouldCopyFileToDestination(string sourceRel, string destRel, string content)
        {
            var sourcePath = Path.Combine(_testDir, sourceRel);
            var destPath = Path.Combine(_testDir, destRel);
            Directory.CreateDirectory(Path.GetDirectoryName(sourcePath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
            await File.WriteAllTextAsync(sourcePath, content);

            await _executor.CopyFile(sourcePath, destPath);

            Assert.True(File.Exists(destPath));
            var copied = await File.ReadAllTextAsync(destPath);
            Assert.Equal(content, copied);
        }

        [Theory]
        [InlineData("source.txt", "moved.txt", "move content")]
        public async Task MoveFile_ShouldMoveFileToDestination(string sourceRel, string destRel, string content)
        {
            var sourcePath = Path.Combine(_testDir, sourceRel);
            var destPath = Path.Combine(_testDir, destRel);
            await File.WriteAllTextAsync(sourcePath, content);

            await _executor.MoveFile(sourcePath, destPath);

            Assert.False(File.Exists(sourcePath));
            Assert.True(File.Exists(destPath));
            var moved = await File.ReadAllTextAsync(destPath);
            Assert.Equal(content, moved);
        }

        [Theory]
        [InlineData("replace.txt", "old text old", "old", "new", "new text new")]
        [InlineData("empty_old.txt", "", "old", "new", "")]
        public async Task ReplaceText_ShouldReplaceAllOccurrences(string fileName, string input, string oldText, string newText, string expected)
        {
            var filePath = Path.Combine(_testDir, fileName);
            await File.WriteAllTextAsync(filePath, input);

            await _executor.ReplaceText(filePath, oldText, newText);
            var result = await File.ReadAllTextAsync(filePath);

            Assert.Equal(expected, result);
        }
    }
}