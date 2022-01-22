using Mochj._Interpreter;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Expressions
{
    public class ExprFnDeclaration : Expression
    {
        public string Name { get; set; }
        public DataType ReturnType { get; set; }
        public IList<StmtParameter> Parameters { get; set; }
        public IList<Statement> Statements { get; set; }
        public ExprFnDeclaration(Location loc)
            : base("ExprFnDeclaration", loc)
        {

        }

        public override QualifiedObject Visit(Interpreter interpreter)
        {
            return interpreter.Accept(this);
        }
    }
}
