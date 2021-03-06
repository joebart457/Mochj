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
    public class ExprGetArgument : Expression
    {
        public Expression Lhs { get; set; }
        public string Identifier { get; set; }
        public bool Strict { get; set; }
        public ExprGetArgument(Location loc)
            : base("ExprGetArgument", loc)
        {
        }

        public ExprGetArgument(Location loc, Expression lhs, bool strict)
            : base("ExprGetArgument", loc)
        {
            Lhs = lhs;
            Strict = strict;
        }

        public override QualifiedObject Visit(Interpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}

