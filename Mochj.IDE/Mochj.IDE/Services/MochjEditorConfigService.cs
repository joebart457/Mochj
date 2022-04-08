using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE.Services
{
    static class MochjEditorConfigService
    {
        public static bool EnableCodeCompletion { get; set; } = true;
        public static bool EnableIntellisenseOnKey { get; set; } = true; 
    }
}
