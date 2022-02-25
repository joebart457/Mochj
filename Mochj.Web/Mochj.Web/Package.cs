using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Web
{
    class RemoteFile
    {
        public string Local { get; set; }
        public string RemoteUrl { get; set; }
    }
    class Package
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string[] Files{ get; set; }
        public RemoteFile[] RemoteFiles { get; set; }
    }
}
