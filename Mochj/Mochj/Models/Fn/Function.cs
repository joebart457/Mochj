using Mochj._Interpreter;
using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj.Builders;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Models.Fn
{
    public class Params
    {
        private IDictionary<int, Parameter> _parametersByPosition = new Dictionary<int, Parameter>();
        private IDictionary<string, Parameter> _parametersByAlias = new Dictionary<string, Parameter>();
        public bool IsVariadic { get; set; } = false;
        public int VariadicAfter { get; set; } = -1;
        public DataType VariadicType { get; set; } = new DataType { TypeId = Enums.DataTypeEnum.Any };
        public Params(IList<Parameter> parameters)
        {
            Construct(parameters);
        }

        public Parameter Get(int pos)
        {
            return _parametersByPosition[pos];
        }

        public Parameter Get(string alias)
        {
            return _parametersByAlias[alias];
        }

        /// <summary>
        /// Takes a list of incomplete arguments and resolves/finalizes them using 
        /// parameters <br></br>
        /// If argument is variadic, an alias is not provided
        /// </summary>
        /// <param name="arguments">List of arguments with defined position and/or alias</param>
        /// <returns></returns>
        public Args ResolveArguments(IList<Argument> arguments)
        {
            IList<Parameter> unresolvedParameters = _parametersByPosition.Values.ToList();
            Args finalArgs = new Args();
            foreach (Argument arg in arguments)
            {
                if (!string.IsNullOrEmpty(arg.Alias))
                {
                    if (_parametersByAlias.ContainsKey(arg.Alias))
                    {
                        finalArgs.Add(PatchArgument(_parametersByAlias[arg.Alias], arg));
                        unresolvedParameters.Remove(_parametersByAlias[arg.Alias]);
                        continue;
                    }
                    throw new Exception($"parameter '{arg.Alias}' does not exist");
                }
                else
                {
                    if (_parametersByPosition.ContainsKey(arg.Position))
                    {
                        finalArgs.Add(PatchArgument(_parametersByPosition[arg.Position], arg));
                        unresolvedParameters.Remove(_parametersByPosition[arg.Position]);
                        continue;
                    } else if (IsVariadic && arg.Position > VariadicAfter)
                    {
                        finalArgs.AddVariadicArgument(PatchVariadicArgument(arg));
                        continue;
                    } 
                    throw new Exception($"argument at position {arg.Position} out of range; parameter does not exist");
                }
            }
            // Resolve remaining parameters
            foreach (Parameter parameter in unresolvedParameters)
            {
                if (parameter.HasDefaultValue())
                {
                    finalArgs.Add(CreateArgument(parameter));
                } else
                {
                    throw new Exception($"unresolved parameter {parameter}");
                }
            }

            return finalArgs;
        }

        private Argument PatchArgument(Parameter parameter, Argument argument)
        {
            Argument finalArgument = new Argument();
            finalArgument.Alias = parameter.Alias;
            finalArgument.Position = parameter.Position;
            finalArgument.Value = TypeHelper.CheckType(argument.Value, parameter.Type);
            return finalArgument;
        }

        private Argument PatchVariadicArgument(Argument argument)
        {
            Argument finalArgument = new Argument();
            finalArgument.Position = argument.Position;
            finalArgument.Value = TypeHelper.CheckType(argument.Value, VariadicType);
            return finalArgument;
        }

        private Argument CreateArgument(Parameter parameter)
        {
            Argument finalArgument = new Argument();
            finalArgument.Alias = parameter.Alias;
            finalArgument.Position = parameter.Position;
            finalArgument.Value = parameter.Default; // Can ignore type check here since we check upon parameter creation
            return finalArgument;
        }

        public IList<Argument> PatchArguments(IList<Argument> arguments)
        {
            IList<Argument> finalArgs = new List<Argument>();
            foreach (Argument arg in arguments)
            {
                if (!string.IsNullOrEmpty(arg.Alias))
                {
                    if (_parametersByAlias.ContainsKey(arg.Alias))
                    {
                        finalArgs.Add(PatchArgument(_parametersByAlias[arg.Alias], arg));
                        continue;
                    }
                    throw new Exception($"unable to bind parameter '{arg.Alias}' does not exist");
                }
                else
                {
                    if (_parametersByPosition.ContainsKey(arg.Position))
                    {
                        finalArgs.Add(PatchArgument(_parametersByPosition[arg.Position], arg));
                        continue;
                    }
                    else if (IsVariadic && arg.Position > VariadicAfter)
                    {
                        finalArgs.Add(PatchVariadicArgument(arg));
                        continue;
                    }
                    throw new Exception($"unable to bind argument at position {arg.Position} out of range; parameter does not exist");
                }
            }          

            return finalArgs;
        }

        private void Construct(IList<Parameter> parameters)
        {
            _parametersByPosition.Clear();
            _parametersByAlias.Clear();

            foreach (Parameter p in parameters)
            {
                if (_parametersByPosition.ContainsKey(p.Position))
                {
                    throw new Exception($"Redefinition of parameter at position {p.Position}");
                }
                if (_parametersByAlias.ContainsKey(p.Alias))
                {
                    throw new Exception($"Redefinition of named parameter {p.Alias}");
                }
                _parametersByPosition.Add(p.Position, p);
                _parametersByAlias.Add(p.Alias, p);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            foreach(Parameter parameter in _parametersByPosition.Values)
            {
                sb.Append($"{parameter}, ");
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public abstract class Function
    {
        public DataType ReturnType { get; set; }
        public string Name { get; set; }
        public Symbol Symbol { get; set; }
        public Params Params { get; set; } = new Params(new List<Parameter>());
        public _Storage.Environment Enclosing { get; set; }
        public abstract QualifiedObject Call(Args args);

        public virtual Args ResolveArguments(IList<Argument> arguments)
        {
            return Params.ResolveArguments(arguments);
        }
        public override string ToString()
        {
            return $"{ReturnType} {Name}({Params})";
        }
    }

    public class BoundFn : Function 
    {
        public Function hFunc { get; set; }
        public Args BoundArguments { get; set; } = new Args();
        public override Args ResolveArguments(IList<Argument> arguments)
        {
            if (hFunc == null)
            {
                throw new NullReferenceException("bound function was null");
            }
            return hFunc.Params.ResolveArguments(BoundArguments.Concat(arguments));
        }

        public override QualifiedObject Call(Args args)
        {
            if (hFunc == null)
            {
                throw new NullReferenceException("bound function was null");
            }
            return hFunc.Call(args);
        }
    }


    public class UserDefinedFunction : Function
    {
        private IList<Statement> _statements;

        public UserDefinedFunction(_Storage.Environment enclosing, StmtFnDeclaration stmtFnDeclaration, IList<Parameter> parameters)
        {
            Params = new Params(parameters);
            Name = stmtFnDeclaration.Name;
            Enclosing = enclosing;
            ReturnType = stmtFnDeclaration.ReturnType;
            _statements = stmtFnDeclaration.Statements;
        }

        public UserDefinedFunction(_Storage.Environment enclosing, ExprFnDeclaration exprFnDeclaration, IList<Parameter> parameters)
        {
            Params = new Params(parameters);
            Name = exprFnDeclaration.Name;
            Enclosing = enclosing;
            ReturnType = exprFnDeclaration.ReturnType;
            _statements = exprFnDeclaration.Statements;
        }
        public override QualifiedObject Call(Args args)
        {
            _Storage.Environment environment = new _Storage.Environment(Enclosing);
            try
            {
                args.DefineInScope(environment);
                Interpreter interpreter = new Interpreter(environment);
                interpreter.Accept(_statements);
                return TypeHelper.CheckType(QualifiedObjectBuilder.BuildEmptyValue(), ReturnType);
            }catch (ReturnException re)
            {
                return TypeHelper.CheckType(re.Value, ReturnType);
            } 
            catch (Exception e)
            {
                throw new Exception($"in call {Name}: {e.Message}");
            }
            
        }
    }

    public class NativeFunction : Function
    {
        private Func<Args, QualifiedObject> _hFunc = null;
        private bool _enforceReturnTypeCheck = true;
        private bool _isVariadic = false;
        private int _variadicAfter = -1;
        private DataType _variadicType = new DataType { TypeId = Enums.DataTypeEnum.Any };
        /// <summary>
        /// Holds parameters while being added
        /// does <b>not</b> affect params object 
        /// until Build() is called
        /// </summary>
        private IList<Parameter> _intermediateParameters = new List<Parameter>();
        public override QualifiedObject Call(Args args)
        {
            if (_hFunc == null)
            {
                throw new NullReferenceException("native function was null");
            } else
            {
                if (!_enforceReturnTypeCheck)
                {
                    return _hFunc(args);
                }
                return TypeHelper.CheckType(_hFunc(args), ReturnType);
            }
        }

        public NativeFunction Action(Func<Args, QualifiedObject> hFunc)
        {
            _hFunc = hFunc;
            return this;
        }

        public NativeFunction VariadicAfter(int index)
        {
            _isVariadic = true;
            _variadicAfter = index;
            return this;
        }

        public NativeFunction VariadicType<Ty>()
        {
            _variadicType = TypeMediatorService.DataType<Ty>();
            return this;
        }
        public NativeFunction ReturnsEmpty()
        {
            ReturnType = new DataType { TypeId = Enums.DataTypeEnum.Empty };
            return this;
        }

        public NativeFunction Returns<Ty>()
        {
            ReturnType = TypeMediatorService.DataType<Ty>();
            return this;
        }

        public NativeFunction RegisterParameter<Ty>(string alias)
        {
            Parameter parameter = new Parameter();
            parameter.Alias = alias;
            parameter.Type = TypeMediatorService.DataType<Ty>();
            parameter.Position = _intermediateParameters.Count;
            _intermediateParameters.Add(parameter);
            return this;
        }

        public NativeFunction RegisterParameter<Ty>(string alias, QualifiedObject defaultValue)
        {
            Parameter parameter = new Parameter();
            parameter.Alias = alias;
            parameter.Type = TypeMediatorService.DataType<Ty>();
            parameter.Position = _intermediateParameters.Count;
            parameter.Default = defaultValue;
            _intermediateParameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Builds all parameters added using
        /// <c>RegisterParameter</c> into <c>Params</c> object
        /// and registers it to the function.
        /// <br></br>
        /// Call after all calls to <c>RegisterParameter</c>
        /// </summary>
        /// <returns></returns>
        public NativeFunction Build()
        {
            Params = new Params(_intermediateParameters);
            if (_isVariadic)
            {
                Params.IsVariadic = true;
                Params.VariadicAfter = _variadicAfter;
                Params.VariadicType = _variadicType;
            }
            
            return this;
        }
    }

}
