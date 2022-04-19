using Mochj._Parser.Models.Expressions;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj.IDE._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Statements
{
    public class StmtParameter
    {
        public Location Loc { get; set; }
        public DataType DataType { get; set; }
        public RangedToken Alias { get; set; }
        public int Position { get; set; }
        public RangedExpression Default { get; set; }

    }
}
