using Mochj._Interpreter;
using Mochj._Parser.Models;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtEntry : RangedStatement
    {
        public Symbol Symbol { get; set; }
        public StmtEntry(Location loc)
            : base("StmtEntry", loc)
        {

        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
