using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Tokenizer.Models
{
    public class Location
    {
        public ulong X { get; set; }
        public ulong Y { get; set; }

        public Location(ulong x, ulong y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"Line {Y}:{X}";
        }
    }
}
