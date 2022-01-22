using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    public abstract class Statement
    {
        public string Label { get; set; }
        public Location Loc { get; set; }

        public Statement(string label, Location loc)
        {
            Loc = loc;
            Label = label;
        }

        public abstract void Visit(Interpreter interpreter);
    }
}
