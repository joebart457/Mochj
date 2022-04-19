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
    public class ExprNullableSwitch : RangedExpression
    {
        public RangedExpression Lhs { get; set; }
        public RangedExpression Rhs { get; set; }
        public ExprNullableSwitch(Location loc)
            : base("ExprNullableSwitch", loc)
        {

        }

        public ExprNullableSwitch(Location loc, RangedExpression lhs, RangedExpression rhs)
            : base("ExprNullableSwitch", loc)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public override QualifiedObject Visit(InformedInterpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
