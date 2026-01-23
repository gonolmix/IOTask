using IOTask.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IOTask
{
    public class FileInformation
    {
        [JsonProperty("path")]
        public string FilePath { get; set; }
        [JsonProperty("action")]
        public FileActions Action { get; set; }
        [JsonProperty("params")]
        public string? Params { get; set; }
    }
}
