using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models.Expressions
{
    public class ExprArgument
    {
        public int Position { get; set; }
        public string ParameterAlias { get; set; }
        public RangedExpression Value { get; set; }

        public Location Loc { get; set; }
        public ExprArgument(Location loc)
        {
            Loc = loc;
        }

    }
}
