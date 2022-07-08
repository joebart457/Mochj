using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Enums
{
    public enum DataTypeEnum: UInt16
    {
        Empty = 0,
        Fn = 1,
        List = 3,
        Boolean = 4,
        Number = 5,
        String = 6,
        Namespace = 7,
        Any = 8,
        NativeList = 9,
        TypeInfo = 10,
        Unknown = 11,
        DateTime = 12,
    }
}
