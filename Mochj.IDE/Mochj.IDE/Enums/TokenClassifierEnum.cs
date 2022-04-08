using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE.Enums
{
    public enum TokenClassifierEnum: UInt16
    {
        Function,
        Namespace,
        Identifier,
        Number,
        String,
        Boolean,
        Unkown,
        Error,
        Warning,

        Keyword_1,
        Keyword_2,
        Keyword_3,
        Keyword_4,
        Comment,
    }
}
