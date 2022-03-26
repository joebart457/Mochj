using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Interpreter.Helpers
{
    public static class TypeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">Any object</param>
        /// <param name="dt">Type to enforce</param>
        /// <returns><c>obj</c> if types match or if dt is <c>Any</c></returns>
        public static QualifiedObject CheckType(QualifiedObject obj, DataType dt)
        {
            if (dt.Equals(obj.Type))
            {
                return obj;
            }
            throw new Exception($"types do not match ({obj.Type} != {dt}) ");
        }
    }
}
