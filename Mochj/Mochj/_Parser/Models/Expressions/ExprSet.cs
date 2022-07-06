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
    public class ExprSet: Expression
    {
        public string Identifier { get; set; }
        public Expression Value { get; set; }

        public ExprSet(Location loc)
                    : base("ExprSet", loc)
        {

        }

        public override QualifiedObject Visit(Interpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
