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
    class StmtSet: Statement
    {
        public string Identifier { get; set; }
        public Expression Value { get; set; }
        public StmtSet(Location loc)
            : base("StmtSet", loc)
        {

        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }

    }
}
