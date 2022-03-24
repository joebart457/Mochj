using Mochj._Interpreter.Helpers;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Storage
{
    public class Environment
    {
        public class EnvironmentSettings
        {
            public bool UseStrictTypes { get; set; } = true;
            public bool FillFunctionSymbols { get; set; } = true;
        }

        private EnvironmentSettings Settings = new EnvironmentSettings();

        private IDictionary<string, QualifiedObject> _lookup = new Dictionary<string, QualifiedObject>();

        public List<KeyValuePair<string, QualifiedObject>> Lookup { get { return _lookup.ToList(); } }

        private Environment _enclosing;
        public Environment Enclosing { get { return _enclosing; } }

        private string _alias = null;
        public string Alias { get; }
        public Environment(Environment enclosing = null)
        {
            _enclosing = enclosing;
        }

        public Environment WithAlias(string alias)
        {
            if (_alias == null)
            {
                _alias = alias; 
            }
            return this;
        }

        public bool Define(string key, QualifiedObject value, bool bOverwrite = true)
        {
            if (bOverwrite)
            {
                AttachSymbol(value);
                _lookup[key] = value;
                return true;
            }
            else
            {
                if (!ExistsLocal(key))
                {
                    AttachSymbol(value);
                    _lookup.Add(key, value);
                    return true;
                }
                return false;
            }
        }

        public bool Assign(string key, QualifiedObject value)
        {
            if (ExistsLocal(key))
            {
                AttachSymbol(value);
                if (Settings.UseStrictTypes)
                {
                    _lookup[key] = TypeHelper.CheckType(value, _lookup[key].Type);
                } else
                {
                    _lookup[key] = value;
                }
                return true;
            }
            else if (Enclosing != null)
            {
                return Enclosing.Assign(key, value);
            }
            return false;
        }

        public bool TryGet(string key, out QualifiedObject value)
        {
            if (_lookup.TryGetValue(key, out value))
            {
                return true;
            }
            else if (_enclosing != null)
            {
                return _enclosing.TryGet(key, out value);
            }
            return false;
        }

        public void Remove(string key)
        {
            _lookup.Remove(key);
        }

        public bool Exists(string key)
        {
            if (_lookup.ContainsKey(key))
            {
                return true;
            }
            else if (Enclosing != null)
            {
                return Enclosing.Exists(key);
            }
            return false;
        }

        public bool ExistsLocal(string key)
        {
            return _lookup.ContainsKey(key);
        }

        public Environment Top()
        {
            if (_enclosing == null)
            {
                return this;
            }
            return _enclosing.Top();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(KeyValuePair<string, QualifiedObject> kv in _lookup)
            {
                string valueString;
                if (kv.Value.Type.Is(Enums.DataTypeEnum.Namespace))
                {
                    valueString = TypeMediatorService.ToNativeType<Environment>(kv.Value).ToString(4);
                }
                else
                {
                    valueString = $"{kv.Value}";
                }
                sb.Append($"{kv.Key} := {valueString}\n");
            }
            return sb.ToString();
        }

        public string ToString(int spaces)
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, QualifiedObject> kv in _lookup)
            {
                string valueString;
                if (kv.Value.Type.Is(Enums.DataTypeEnum.Namespace))
                {
                    valueString = TypeMediatorService.ToNativeType<_Storage.Environment>(kv.Value).ToString(spaces + 4);
                } else
                {
                    valueString = $"{kv.Value}";
                }
                sb.Append($"{new string(' ', spaces)}{kv.Key} := {valueString}\n");
            }
            return sb.ToString();
        }

        private void AttachSymbol(QualifiedObject obj)
        {
            if (Settings.FillFunctionSymbols && obj.Type.Is(Enums.DataTypeEnum.Fn))
            {
                var envPath = GetFullPath();
                var fn = TypeMediatorService.ToNativeType<Models.Fn.Function>(obj);
                if (fn.Symbol != null) return;
                envPath.Add(fn.Name);
                fn.Symbol = new _Parser.Models.Symbol { Names = envPath };
            }
        }

        private List<string> GetFullPath()
        {
            List<string> aliases = new List<string>();
            if (_enclosing != null)
            {
                aliases.AddRange(_enclosing.GetFullPath());
            }
            if (!string.IsNullOrEmpty(_alias)) aliases.Add(_alias);

            return aliases;
        }
    }
}
