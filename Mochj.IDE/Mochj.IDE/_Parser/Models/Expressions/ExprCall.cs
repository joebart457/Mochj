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
    public class ExprCall : RangedExpression
    {
        public Symbol Symbol { get; set; }
        public IList<ExprArgument> Arguments { get; set; }
        public ExprCall(Location loc)
            : base("ExprCall", loc)
        {

        }

        public override QualifiedObject Visit(InformedInterpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
