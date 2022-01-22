using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models
{
    public class Symbol
    {
        public Location Location { get; set; }
        public IList<string> Names { get; set; }

        public override string ToString()
        {
            if (Names == null)
            {
                return "";
            }
            return string.Join(".", Names);
        }
    }
}
