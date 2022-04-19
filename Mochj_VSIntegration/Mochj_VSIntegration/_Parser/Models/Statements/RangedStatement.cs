using Microsoft.VisualStudio.Text;
using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Mochj.IDE._Parser.Models.Statements
{
    public abstract class RangedStatement
    {
        public string Label { get; set; }
        public Location Loc { get; set; }

        public Span TextRange { get; set; }
        public RangedStatement(string label, Location loc)
        {
            Loc = loc;
            Label = label;
        }

        public abstract void Visit(InformedInterpreter interpreter);
    }
}
