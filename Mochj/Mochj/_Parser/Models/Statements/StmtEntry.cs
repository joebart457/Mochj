using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    class StmtEntry : Statement
    {
        public Symbol Symbol { get; set; }
        public StmtEntry(Location loc)
            : base("StmtEntry", loc)
        {

        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
