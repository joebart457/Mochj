using Mochj.Builders;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj
{
    class Program
    {
        static int Main(string[] args)
        {
            return ProgramStartupService.Startup(args);
        }
    }
}
