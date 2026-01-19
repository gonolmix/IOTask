using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IOTask
{
    public class JsonSettings
    {
        [JsonProperty("files")]
        public List<FileProcessor> Files { get; set; } = new();

        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
