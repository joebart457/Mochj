using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._PackageManager.Models.Constants
{
    public static class PathConstants
    {
        public static readonly string StagePath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/stage/";
        public static readonly string PackagePath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/";

    }
}
