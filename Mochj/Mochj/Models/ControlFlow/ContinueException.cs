using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models.ControlFlow
{
    class ContinueException : Exception
    {
        public ContinueException(Location loc)
            : base($"continue exception at {loc}")
        {
        }

        public ContinueException()
            : base($"uncaught continue")
        {
        }
    }
}
