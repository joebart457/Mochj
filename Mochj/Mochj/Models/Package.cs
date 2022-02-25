using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    class RemoteFile
    {
        public string Local { get; set; }
        [JsonProperty("remote_url")]
        public string RemoteUrl { get; set; }
    }
    class Package
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string[] Files { get; set; }
        public RemoteFile[] RemoteFiles { get; set; }
    }
}
