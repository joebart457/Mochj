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

        private IDictionary<string, QualifiedObject> _lookup = new Dictionary<string, QualifiedObject>();

        private Environment _enclosing;
        public Environment Enclosing { get { return _enclosing; } }

        public Environment(Environment enclosing = null)
        {
            _enclosing = enclosing;
        }
        public bool Define(string key, QualifiedObject value, bool bOverwrite = true)
        {
            if (bOverwrite)
            {
                _lookup.Add(key, value);
                return true;
            }
            else
            {
                if (!ExistsLocal(key))
                {
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
                _lookup[key] = value;
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
    }
}
