using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    class ExitException : Exception
    {
        public Location Loc { get; set; }
        public int Value { get; set; }
        public ExitException(int value, Location loc)
            : base($"({value}) exit requested at {loc}")
        {
            Loc = loc;
            Value = value;
        }

        public ExitException(int value)
            : base($"({value}) exit requested")
        {
            Value = value;
        }
    }
}
