using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models
{
    class PackageSettings
    {
        public bool PublishToLocal { get; set; } = false;
        public string OutputDirectory { get; set; }
        public string ManifestPath { get; set; }
        public bool UsePreviousLoadFiles { get; set; } = false;
        public List<PackageInfo> PackageInfo { get; set; } = new List<PackageInfo>();
    }
}
