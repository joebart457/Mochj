using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models.Fn
{
    class ReturnException: Exception
    {
        public Location Loc { get; set; }
        public QualifiedObject Value { get; set; }
        public Function Fn { get; set; } = null;
        public ReturnException(QualifiedObject value, Function fn, Location loc)
            :base($"return exception at {loc}")
        {
            Loc = loc;
            Value = value;
            Fn = fn;
        }

        public ReturnException(QualifiedObject value, Function fn)
            : base($"return exception")
        {
            Value = value;
            Fn = fn;
        }
    }
}
