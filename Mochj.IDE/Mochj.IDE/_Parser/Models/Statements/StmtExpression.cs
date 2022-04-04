using Mochj._Interpreter;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mochj.IDE._Interpreter;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtExpression: RangedStatement
    {
        public RangedExpression Expression { get; set; }
        public StmtExpression(Location loc)
            : base("StmtExpression", loc)
        {

        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
