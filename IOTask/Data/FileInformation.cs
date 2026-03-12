using IOTask.Enums;
using Newtonsoft.Json;

namespace IOTask
{
    public class FileInformation
    {
        [JsonProperty("path")]
        public string FilePath { get; set; }

        [JsonProperty("action")]
        public FileActions Action { get; set; }

        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("destinationPath")]
        public string? DestinationPath { get; set; }

        [JsonProperty("oldText")]
        public string? OldText { get; set; }

        [JsonProperty("newText")]
        public string? NewText { get; set; }
    }
}
