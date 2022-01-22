using Mochj._Parser.Models.Expressions;
using Mochj._Tokenizer.Models;
using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    public class StmtParameter
    {
        public Location Loc { get; set; }
        public DataType DataType { get; set; }
        public string Alias { get; set; }
        public int Position { get; set; }
        public Expression Default { get; set; }

    }
}
