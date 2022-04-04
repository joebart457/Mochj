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
    public class StmtNamespace : RangedStatement
    {
        public Symbol Symbol { get; set; }
        public StmtNamespace(Location loc)
            : base("StmtNamespace", loc)
        {

        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
