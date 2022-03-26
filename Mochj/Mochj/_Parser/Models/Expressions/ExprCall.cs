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
    public class ExprCall : Expression
    {
        public Symbol Symbol { get; set; }
        public IList<ExprArgument> Arguments { get; set; }
        public ExprCall(Location loc)
            : base("ExprCall", loc)
        {

        }

        public override QualifiedObject Visit(Interpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
