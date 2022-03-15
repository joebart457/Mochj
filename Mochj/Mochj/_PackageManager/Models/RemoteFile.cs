using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models
{
    class RemoteFile
    {
        public string Local { get; set; }
        [JsonProperty("remote_url")]
        public string RemoteUrl { get; set; }
    }
}
