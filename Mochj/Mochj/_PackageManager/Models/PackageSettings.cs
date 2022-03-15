using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models
{
    class PackageSettings
    {
        public string PackageName { get; set; }
        public List<string> IncludedFiles { get; set; } = new List<string>();
        public string VersionRecordPath { get; set; }
    }
}
