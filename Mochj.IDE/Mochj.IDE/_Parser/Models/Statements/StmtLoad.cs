using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtLoad : RangedStatement
    {
        public RangedToken Path { get; set; }
        public StmtLoad(Location loc)
            : base("StmtLoad", loc)
        {

        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
