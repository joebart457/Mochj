using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtFnDeclaration : RangedStatement
    {
        public RangedToken Name { get; set; }
        public DataType ReturnType { get; set; }
        public IList<StmtParameter> Parameters { get; set; }
        public IList<RangedStatement> Statements { get; set; }
        public StmtFnDeclaration(Location loc)
            : base("StmtFnDeclaration", loc)
        {
        }

        public override void Visit(InformedInterpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
