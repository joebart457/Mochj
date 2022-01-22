using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    public class StmtFnDeclaration : Statement
    {
        public string Name { get; set; }
        public DataType ReturnType { get; set; }
        public IList<StmtParameter> Parameters { get; set; }
        public IList<Statement> Statements { get; set; }
        public StmtFnDeclaration(Location loc)
            : base("StmtFnDeclaration", loc)
        {
        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
