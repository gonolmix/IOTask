using Newtonsoft.Json;

namespace IOTask.Data
{
    public class JsonSettings
    {
        [JsonProperty("files")]
        public List<FileInformation> Files { get; set; } = new();

        [JsonProperty("delay")]
        public int Delay { get; set; }

        [JsonProperty("baseDirectory")]

        public string BaseDirectory { get; set; } = "D:/Tests/";
    }
}
