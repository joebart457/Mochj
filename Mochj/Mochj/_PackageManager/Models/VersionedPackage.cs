using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models
{
    class VersionedPackage
    {
        public string Name { get; set; }
        public List<RemotePackage> Versions { get; set; } = new List<RemotePackage>();
    }
}
