using IOTask;
using IOTask.Data;
using IOTask.Enums;
using IOTask.FileProcessorClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTests
{
    public class JsonDeserializationTests
    {
        [Fact]
        public async Task EndToEnd_SuccessfulReplace()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            var testFile = Path.Combine(tempDir, "replace_test.txt");
            await File.WriteAllTextAsync(testFile, "Hello old world");

            var fileInfo = new FileInformation
            {
                FilePath = testFile,
                Action = FileActions.REPLACE,
                OldText = "old",
                NewText = "new"
            };

            var logger = new TestLogger();
            var executor = new FileActionExecutor();
            var processor = new FileProcessor(fileInfo, logger, executor, tempDir);

            try
            {
                await processor.RunAsync(0);

                var content = await File.ReadAllTextAsync(testFile);
                Assert.Equal("Hello new world", content);

                Assert.Contains("SUCCESS [REPLACE]", logger.LogContent);
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }
        [Fact]
        public void Deserialize_MissingFilesSection_ShouldSetToEmptyList()
        {
            var json = """{ "delay": 1000 }""";
            var settings = JsonConvert.DeserializeObject<JsonSettings>(json);

            Assert.NotNull(settings);
            Assert.Empty(settings.Files);
        }

        [Fact]
        public void Deserialize_EmptyJson_ShouldReturnNull()
        {
            var json = "";
            var result = JsonConvert.DeserializeObject<JsonSettings>(json);

            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_WhitespaceJson_ShouldReturnNull()
        {
            var json = "   \t\n  ";
            var result = JsonConvert.DeserializeObject<JsonSettings>(json);

            Assert.Null(result);
        }

        [Fact]
        public void Deserialize_InvalidJsonSyntax_ShouldThrow()
        {
            var json = "{ files: [ }";
            Assert.Throws<JsonReaderException>(() =>
                JsonConvert.DeserializeObject<JsonSettings>(json));
        }

        [Fact]
        public void Deserialize_NullFilePath_ShouldBeHandledInValidation()
        {
            var json = """
        {
          "files": [{ "path": null, "action": "READ" }],
          "delay": 1000
        }
        """;
            var settings = JsonConvert.DeserializeObject<JsonSettings>(json);

            Assert.Null(settings.Files[0].FilePath);

            var processor = new FileProcessor(
                settings.Files[0],
                new TestLogger(),
                new FileActionExecutor(),
                "./"
            );

            var ex = Assert.ThrowsAsync<ArgumentException>(() => processor.RunAsync(0));
            Assert.Contains("FilePath", ex.Result.Message);
        }
    }
}
