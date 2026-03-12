using IOTask;
using IOTask.Enums;
using IOTask.FileProcessorClass;
using System.Security;

namespace IOTests
{
    public class FileProcessorTests : IAsyncLifetime
    {
        private readonly string _sandboxDir;
        private readonly string _outsideDir;
        private readonly FileActionExecutor _executor;

        public FileProcessorTests()
        {
            var baseTemp = Path.Combine(Path.GetTempPath(), "IOTaskTests_" + Guid.NewGuid().ToString());
            _sandboxDir = Path.Combine(baseTemp, "sandbox");
            _outsideDir = Path.Combine(baseTemp, "outside");
            Directory.CreateDirectory(_sandboxDir);
            Directory.CreateDirectory(_outsideDir);
            _executor = new FileActionExecutor();
        }

        public async Task DisposeAsync()
        {
            var parent = Directory.GetParent(_sandboxDir)!.FullName;
            if (Directory.Exists(parent))
                Directory.Delete(parent, recursive: true);
            await Task.CompletedTask;
        }

        public Task InitializeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("inside.txt", true)]
        [InlineData("sub/inside.txt", true)]
        [InlineData("../outside.txt", false)]
        [InlineData("..\\outside.txt", false)]
        [InlineData("/absolute/path.txt", false)]
        public async Task RunAsync_ShouldValidatePathSecurity(string inputPath, bool shouldSucceed)
        {
            var fileInfo = new FileInformation
            {
                FilePath = inputPath,
                Action = FileActions.DELETE
            };
            var logger = new TestLogger();
            var processor = new FileProcessor(fileInfo, logger, _executor, _sandboxDir);

            if (shouldSucceed)
            {
                await Assert.ThrowsAsync<FileNotFoundException>(() => processor.RunAsync(0));
            }
            else
            {
                var ex = await Assert.ThrowsAsync<ArgumentException>(() => processor.RunAsync(0));
                Assert.Contains("outside base directory", ex.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData(FileActions.COPY)]
        [InlineData(FileActions.MOVE)]
        public async Task RunAsync_ShouldRequireDestinationPathForCopyMove(FileActions action)
        {
            var fileInfo = new FileInformation
            {
                FilePath = Path.Combine(_sandboxDir, "source.txt"),
                Action = action
            };
            var logger = new TestLogger();
            var processor = new FileProcessor(fileInfo, logger, _executor, _sandboxDir);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => processor.RunAsync(0));
            Assert.Contains("destinationPath", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData(FileActions.REPLACE, null, "new", false)]   
        [InlineData(FileActions.REPLACE, "", "new", false)]     
        [InlineData(FileActions.REPLACE, "old", null, true)]    
        [InlineData(FileActions.REPLACE, "old", "", true)]      
        [InlineData(FileActions.REPLACE, "old", "new", true)]   
        public async Task RunAsync_ShouldValidateReplaceParameters(FileActions action, string? oldText, string? newText, bool shouldSucceed)
        {
            var fileInfo = new FileInformation
            {
                FilePath = Path.Combine(_sandboxDir, "test.txt"),
                Action = action,
                OldText = oldText,
                NewText = newText
            };
            var logger = new TestLogger();
            var processor = new FileProcessor(fileInfo, logger, _executor, _sandboxDir);

            if (shouldSucceed)
            {
                await Assert.ThrowsAsync<FileNotFoundException>(() => processor.RunAsync(0));
            }
            else
            {
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => processor.RunAsync(0));
                Assert.Contains("oldText", ex.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Theory]
        [InlineData(-100, false)]
        [InlineData(0, true)]
        [InlineData(1000, true)]
        public async Task RunAsync_ShouldValidateDelay(int delay, bool shouldSucceed)
        {
            var fileInfo = new FileInformation
            {
                FilePath = Path.Combine(_sandboxDir, "test.txt"),
                Action = FileActions.READ
            };
            var logger = new TestLogger();
            var processor = new FileProcessor(fileInfo, logger, _executor, _sandboxDir);

            if (shouldSucceed)
            {
                await Assert.ThrowsAsync<FileNotFoundException>(() => processor.RunAsync(delay));
            }
            else
            {
                var ex = await Assert.ThrowsAsync<ArgumentException>(() => processor.RunAsync(delay));
                Assert.Contains("Delay", ex.Message, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public async Task RunAsync_ShouldLogSuccessAndError()
        {
            var existingFile = Path.Combine(_sandboxDir, "exists.txt");
            await File.WriteAllTextAsync(existingFile, "content");

            var nonExistingFile = Path.Combine(_sandboxDir, "does_not_exist.txt");

            var logger = new TestLogger();
            var executor = new FileActionExecutor();

            var successOp = new FileInformation { FilePath = existingFile, Action = FileActions.DELETE };
            var successProc = new FileProcessor(successOp, logger, executor, _sandboxDir);
            await successProc.RunAsync(0);

            var errorOp = new FileInformation { FilePath = nonExistingFile, Action = FileActions.DELETE };
            var errorProc = new FileProcessor(errorOp, logger, executor, _sandboxDir);
            await Assert.ThrowsAsync<FileNotFoundException>(() => errorProc.RunAsync(0));

            var log = logger.LogContent;
            Assert.Contains("SUCCESS [DELETE]", log);
            Assert.Contains("ERROR [DELETE]", log);
        }
    }
}
