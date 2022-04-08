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
    public class ExprNullableSwitch : Expression
    {
        public Expression Lhs { get; set; }
        public Expression Rhs { get; set; }
        public ExprNullableSwitch(Location loc)
            : base("ExprNullableSwitch", loc)
        {

        }

        public ExprNullableSwitch(Location loc, Expression lhs, Expression rhs)
            : base("ExprNullableSwitch", loc)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public override QualifiedObject Visit(Interpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
