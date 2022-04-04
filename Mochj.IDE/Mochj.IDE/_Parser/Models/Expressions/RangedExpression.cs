using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Mochj.IDE._Parser.Models.Expressions
{
    public abstract class RangedExpression
    {
        public string Label { get; set; }
        public Location Loc { get; set; }
        public TextRange TextRange { get; set; }
        public RangedExpression(string label, Location loc)
        {
            Loc = loc;
            Label = label;
        }

        public abstract QualifiedObject Visit(InformedInterpreter interpreter);
    }
}
