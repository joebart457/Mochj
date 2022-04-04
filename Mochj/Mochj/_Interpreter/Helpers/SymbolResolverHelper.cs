using Mochj._Parser.Models;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Interpreter.Helpers
{
    public static class SymbolResolverHelper
    {

        public static QualifiedObject Resolve(_Storage.Environment environment, Symbol symbol)
        {
            QualifiedObject result = QualifiedObjectBuilder.BuildEmptyValue();
            _Storage.Environment target = environment;
            foreach (string name in symbol.Names)
            {
                if (!target.TryGet(name, out result))
                {
                    throw new Exception($"unresolved symbol {name} at {symbol.Location}");
                }
                if (result.Type.Is(Enums.DataTypeEnum.Namespace))
                {
                    if (result.Object is _Storage.Environment env)
                    {
                        target = env;
                    }
                    else
                    {
                        throw new Exception($"object {result} is not convertible to namespace");
                    }
                }
            }
            return result;
        }

        public static bool Resolvable(_Storage.Environment environment, string alias)
        {
            return environment.Exists(alias);
        }

        public static _Storage.Environment ResolveToNamespace(_Storage.Environment environment, Symbol symbol)
        {
            _Storage.Environment current = environment;
            foreach(string name in symbol.Names)
            {
                if (current.ExistsLocal(name))
                {
                    QualifiedObject result;
                    if (current.TryGet(name, out result) && result.Type.Is(Enums.DataTypeEnum.Namespace))
                    {
                        current = TypeMediatorService.ToNativeType<_Storage.Environment>(result);
                        continue;
                    }
                    throw new Exception($"unable to resolve {name} in {symbol} to namespace");
                } else
                {
                    _Storage.Environment newNamespace = new _Storage.Environment(current).WithAlias(name);
                    current.Define(name, QualifiedObjectBuilder.BuildNamespace(newNamespace));
                    current = newNamespace;
                }
            }
            return current;
        }

        public static _Storage.Environment ResolveToNamespaceOrNull(_Storage.Environment environment, Symbol symbol)
        {
            if (symbol == null || symbol.Names == null || !symbol.Names.Any()) return environment;
            _Storage.Environment current = environment;
            foreach (string name in symbol.Names)
            {
                if (current.ExistsLocal(name))
                {
                    QualifiedObject result;
                    if (current.TryGet(name, out result) && result.Type.Is(Enums.DataTypeEnum.Namespace))
                    {
                        current = TypeMediatorService.ToNativeType<_Storage.Environment>(result);
                        continue;
                    }
                    return null;
                }
                else
                {
                    return null;
                }
            }
            return current;
        }
    }
}
