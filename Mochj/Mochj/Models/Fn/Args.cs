using Mochj._Interpreter.Helpers;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models.Fn
{
    public class Argument
    {
        public string Alias { get; set; } = "";
        public int Position { get; set; }
        public QualifiedObject Value { get; set; }
    }
    public class Args
    {
        private IDictionary<int, Argument> _argumentsByPosition = new Dictionary<int, Argument>();
        private IDictionary<string, Argument> _argumentsByAlias = new Dictionary<string, Argument>();
        public Args() { }

        public Args(IList<Argument> arguments)
        {
            foreach(Argument argument in arguments)
            {
                Add(argument);
            }
        }

        public void Replace(string alias, QualifiedObject value)
        {
            if (!_argumentsByAlias.ContainsKey(alias))
            {
                throw new Exception($"argument named '{alias}' does not exist");
            }
            _argumentsByAlias[alias].Value = TypeHelper.CheckType(value, _argumentsByAlias[alias].Value.Type);
        }

        public void Replace(int position, QualifiedObject value)
        {
            if (!_argumentsByPosition.ContainsKey(position))
            {
                throw new Exception($"argument at position {position} does not exist");
            }
            _argumentsByPosition[position].Value = TypeHelper.CheckType(value, _argumentsByPosition[position].Value.Type);
        }

        public IList<Argument> Concat(IList<Argument> arguments)
        {
            return ToList().Concat(arguments).ToList();
        }

        public IList<Argument> ToList()
        {
            return _argumentsByPosition.Values.ToList();
        }

        public IList<Argument> ToList(int excludePosition)
        {
            return _argumentsByPosition.Values
                .Where(arg => arg.Position != excludePosition)
                .Select(arg => {
                    Argument finalArg = new Argument();
                    finalArg.Alias = arg.Alias;
                    finalArg.Position = arg.Position > excludePosition ? arg.Position - 1 : arg.Position;
                    finalArg.Value = arg.Value;
                    return finalArg;
                }).ToList();
        }

        public void Add(Argument argument)
        {
            if (_argumentsByAlias.ContainsKey(argument.Alias))
            {
                throw new Exception($"argument named '{argument.Alias}' was already provided");
            }
            if (_argumentsByPosition.ContainsKey(argument.Position))
            {
                throw new Exception($"argument at position {argument.Position} was already provided");
            }
            _argumentsByAlias.Add(argument.Alias, argument);
            _argumentsByPosition.Add(argument.Position, argument);
        }

        /// <summary>
        /// Adds a variadic argument, ignoring alias checks.<br></br>
        /// Does not add to argumentByAlias lookup.
        /// </summary>
        /// <param name="argument">A variadic argument with position defined</param>
        public void AddVariadicArgument(Argument argument)
        {
            if (_argumentsByPosition.ContainsKey(argument.Position))
            {
                throw new Exception($"argument at position {argument.Position} was already provided");
            }
            _argumentsByPosition.Add(argument.Position, argument);
        }

        public void DefineInScope(_Storage.Environment environment)
        {
            IList<Argument> allArgs = _argumentsByPosition.Values.ToList();
            foreach(Argument arg in allArgs)
            {
                if (!environment.Define(arg.Alias, arg.Value, false))
                {
                    throw new ArgumentException($"argument {arg} is already defined in this scope");
                }
            }
        }

        public QualifiedObject Get(string alias)
        {
            if (_argumentsByAlias.ContainsKey(alias))
            {
                return _argumentsByAlias[alias].Value;
            }
            throw new Exception($"argument with name {alias} is not defined");
        }

        public QualifiedObject Get(int position)
        {
            if (_argumentsByPosition.ContainsKey(position))
            {
                return _argumentsByPosition[position].Value;
            }
            throw new Exception($"argument at {position} is not defined; position out of range");
        }

        public Ty Get<Ty>(string alias)
        {
            if (_argumentsByAlias.ContainsKey(alias))
            {
                return TypeMediatorService.ToNativeType<Ty>(_argumentsByAlias[alias].Value);
            }
            throw new Exception($"argument with name {alias} is not defined");
        }

        public Ty Get<Ty>(int position)
        {
            if (_argumentsByPosition.ContainsKey(position))
            {
                return TypeMediatorService.ToNativeType<Ty>(_argumentsByPosition[position].Value);
            }
            throw new Exception($"argument at {position} is not defined; position out of range");
        }

        public Ty GetOrDefault<Ty>(string alias, Ty _default)
        {
            try
            {
                return TypeMediatorService.ToNativeType<Ty>(_argumentsByAlias[alias].Value);
            } catch (Exception)
            {
                return _default;
            }
        }

        public Ty GetOrDefault<Ty>(int position, Ty _default)
        {
            try
            {
                return TypeMediatorService.ToNativeType<Ty>(_argumentsByPosition[position].Value);
            } catch (Exception)
            {
                return _default;
            }
        }

        public bool ContainsKey(string alias)
        {
            return _argumentsByAlias.ContainsKey(alias);
        }

    }
}
