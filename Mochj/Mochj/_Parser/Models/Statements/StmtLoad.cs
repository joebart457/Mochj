using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    class StmtLoad : Statement
    {
        public string Path { get; set; }
        public StmtLoad(Location loc)
            : base("StmtLoad", loc)
        {

        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
