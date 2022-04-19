using Mochj._Interpreter;
using Mochj._Parser.Models.Expressions;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj.IDE._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtSet : RangedStatement
    {
        public RangedToken Identifier { get; set; }
        public RangedExpression Value { get; set; }
        public StmtSet(Location loc)
            : base("StmtSet", loc)
        {

        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }

    }
}
