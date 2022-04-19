using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Expressions
{
    public class ExprGetArgument : RangedExpression
    {
        public RangedExpression Lhs { get; set; }
        public string Identifier { get; set; }
        public ExprGetArgument(Location loc)
            : base("ExprGetArgument", loc)
        {
        }

        public ExprGetArgument(Location loc, RangedExpression lhs)
            : base("ExprGetArgument", loc)
        {
            Lhs = lhs;
        }

        public override QualifiedObject Visit(InformedInterpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
