using Mochj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Interpreter.Helpers
{
    static class StorageHelper
    {
        public static void AssignStrict(_Storage.Environment environment, string alias, QualifiedObject value)
        {
            QualifiedObject oldValue;
            if (!environment.TryGet(alias, out oldValue))
            {
                throw new Exception($"undefined alias {alias}");
            }
            if (!oldValue.Type.Equals(value.Type))
            {
                throw new Exception($"unable to assign object of type {value.Type} to object of type {oldValue.Type}");
            }
            if (!environment.Assign(alias, value))
            {
                throw new Exception($"error occurred assigning value {value} to symbol {alias}");
            }
        }

        public static void Define(_Storage.Environment environment, string alias, QualifiedObject value)
        {
            if (!environment.Define(alias, value, false))
            {
                throw new Exception($"unable to define {alias} in current scope");
            }
        }
    }
}
