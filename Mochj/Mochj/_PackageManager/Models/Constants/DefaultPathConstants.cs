using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models.Constants
{
    public static class DefaultPathConstants
    {
        public static readonly string ManifestPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json");
        public static readonly string RemoteInfoPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "remote.json");
    }
}
