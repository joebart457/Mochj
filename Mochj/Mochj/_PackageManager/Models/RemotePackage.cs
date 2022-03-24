using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models
{
    class RemotePackage
    {
        public string VersionNumber { get; set; }
        public string RemoteUrl { get; set; }
        public string LocalPath { get; set; }
        public string Hash { get; set; }
        public List<string> LoadFiles { get; set; } = new List<string>();
    }
}
