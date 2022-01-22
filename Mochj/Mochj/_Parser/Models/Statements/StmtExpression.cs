using Mochj._Interpreter;
using Mochj._Parser.Models.Expressions;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    class StmtExpression: Statement
    {
        public Expression Expression { get; set; }
        public StmtExpression(Location loc)
            : base("StmtExpression", loc)
        {

        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
