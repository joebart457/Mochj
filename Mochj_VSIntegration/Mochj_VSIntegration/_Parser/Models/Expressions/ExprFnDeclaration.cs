using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Interpreter;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mochj.IDE._Parser.Models.Statements;
using Mochj.IDE._Tokenizer.Models;

namespace Mochj.IDE._Parser.Models.Expressions
{
    public class ExprFnDeclaration : RangedExpression
    {
        public RangedToken Name { get; set; }
        public DataType ReturnType { get; set; }
        public IList<StmtParameter> Parameters { get; set; }
        public IList<RangedStatement> Statements { get; set; }
        public ExprFnDeclaration(Location loc)
            : base("ExprFnDeclaration", loc)
        {

        }

        public override QualifiedObject Visit(InformedInterpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
