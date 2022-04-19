using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Expressions
{
    public class ExprIdentifier : RangedExpression
    {
        public Symbol Symbol { get; set; }
        public ExprIdentifier(Location loc)
            : base("ExprIdentifier", loc)
        {

        }

        public override QualifiedObject Visit(InformedInterpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
