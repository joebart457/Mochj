using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models
{
    public class QualifiedObject
    {
        public DataType Type { get; set; }
        public object Object { get; set; }

        public bool Equals(QualifiedObject obj)
        {
            if (obj == null) return false;
            return Type.Equals(obj.Type) && ( (Object == null && obj.Object == null) || Object.Equals(obj.Object));
        }

        public override string ToString()
        {
            return $"<Object of type: {Type} with value: {Object}>";
        }
    }
}
