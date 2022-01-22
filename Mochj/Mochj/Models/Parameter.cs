using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    public class Parameter
    {
        public int Position { get; set; }
        public DataType Type { get; set; }
        public string Alias { get; set; }
        public QualifiedObject Default { get; set; }

        public bool HasDefaultValue()
        {
            return Default != null;
        }

        public override string ToString()
        {
            return $"{Type} {Alias}[{Position}]";
        }
    }
}
