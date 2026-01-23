using IOTask.FileProcessorClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace IOTask.Data
{
    public class JsonSettings
    {
        [JsonProperty("files")]
        public List<FileInformation> Files { get; set; } = new();

        [JsonProperty("delay")]
        public int Delay { get; set; }
    }
}
