using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Expressions
{
    public abstract class Expression
    {
        public string Label { get; set; }
        public Location Loc { get; set; }

        public Expression(string label, Location loc)
        {
            Loc = loc;
            Label = label;
        }

        public abstract QualifiedObject Visit(Interpreter interpreter);
    }
}
