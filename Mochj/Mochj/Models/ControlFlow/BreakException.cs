using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models.ControlFlow
{
    class BreakException : Exception
    {
        public BreakException(Location loc)
            : base($"break exception at {loc}")
        {
        }

        public BreakException()
            : base($"uncaught break")
        {
        }

    }
}
