using Mochj._Tokenizer.Models;
using Mochj.IDE._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Models
{
    public class Symbol
    {
        public Location Loc { get; set; }
        public List<RangedToken> Names { get; set; }
    }
}
