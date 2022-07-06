using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Constants;
using Mochj.Models.ControlFlow;
using Mochj.Models.Fn;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Interpreter
{
    public class Interpreter
    {
        private _Storage.Environment _environment;
        private Symbol _entryPoint;
        public Interpreter(_Storage.Environment environment)
        {
            if (environment == null)
            {
                _environment = new _Storage.Environment(null);
            } else
            {
                _environment = environment;
            }
        }

        public Function GetEntryPoint()
        {
            if (_entryPoint == null)
            {
                return null;
            }
            try
            {
                QualifiedObject entry = SymbolResolverHelper.Resolve(_environment.Top(), _entryPoint);
                return TypeMediatorService.ToNativeType<Function>(entry);
            } catch(Exception e)
            {
                throw new Exception($"Unable to resolve symbol {_entryPoint} to functional entry point. Error: {e.Message}");
            }
        }


        public virtual void Accept(IEnumerable<Statement> statements)
        {
            foreach(Statement statement in statements)
            {
                Accept(statement);
            }
        }

        public virtual void Accept(Statement statement)
        {
            statement.Visit(this);
        }

        public virtual void Accept(StmtNamespace stmtNamespace)
        {
            _environment = SymbolResolverHelper.ResolveToNamespace(_environment.Top(), stmtNamespace.Symbol);
        }

        public virtual void Accept(StmtLoad stmtLoad)
        {
            string path = stmtLoad.Path;
            if (!File.Exists(path))
            {
                path = LoadFileHelper.SwitchPathToExecutableHome(path);
                if (!File.Exists(path))
                {
                    throw new Exception($"unable to open {stmtLoad.Path} location: {stmtLoad.Loc}");
                }
            }
            LoadFileHelper.LoadFile(_environment.Top(), path);
        }

        public virtual void Accept(StmtEntry stmtEntry)
        {
            _entryPoint = stmtEntry.Symbol;
        }

        public virtual void Accept(StmtSet stmtSet)
        {
            if (SymbolResolverHelper.Resolvable(_environment, stmtSet.Identifier))
            {
                StorageHelper.AssignStrict(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            } else
            {
                StorageHelper.Define(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
        }

        public virtual void Accept(StmtFnDeclaration stmtFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (StmtParameter stmtParameter in stmtFnDeclaration.Parameters)
            {
                resolvedParameters.Add(ResolveParameter(stmtParameter));
            }
            Function fn = new UserDefinedFunction(_environment, stmtFnDeclaration, resolvedParameters);
            StorageHelper.Define(_environment, fn.Name, QualifiedObjectBuilder.BuildFunction(fn));
        }

        public virtual Parameter ResolveParameter(StmtParameter stmtParameter)
        {
            Parameter parameter = new Parameter();
            parameter.Alias = stmtParameter.Alias;
            parameter.Type = stmtParameter.DataType;
            parameter.Position = stmtParameter.Position;
            if (stmtParameter.Default != null)
            {
                parameter.Default = TypeHelper.CheckType(Accept(stmtParameter.Default), parameter.Type);
            }
            return parameter;
        }

        public virtual void Accept(StmtExpression stmtExpression)
        {
            Accept(stmtExpression.Expression);
        }

        public virtual QualifiedObject Accept(Expression expression)
        {
            return expression.Visit(this);
        }

        public virtual QualifiedObject Accept(ExprSet exprSet)
        {
            if (SymbolResolverHelper.Resolvable(_environment, exprSet.Identifier))
            {
                var value = Accept(exprSet.Value);
                StorageHelper.AssignStrict(_environment, exprSet.Identifier, value);
                return value;
            }
            else
            {
                var value = Accept(exprSet.Value);
                StorageHelper.Define(_environment, exprSet.Identifier, value);
                return value;
            }
        }

        public virtual QualifiedObject Accept(ExprCall exprCall)
        {
            QualifiedObject callable = SymbolResolverHelper.Resolve(_environment, exprCall.Symbol);
            if (!callable.Type.Is(Enums.DataTypeEnum.Fn))
            {
                throw new Exception($"[symbol:{exprCall.Symbol}] expected object of type '{Enums.DataTypeEnum.Fn}' but got '{callable.Type}' location: {exprCall.Loc}");
            }
            if (callable.Object is Models.Fn.Function fn)
            {
                IList<Argument> arguments = new List<Argument>();
                foreach(ExprArgument exprArgument in exprCall.Arguments)
                {
                    arguments.Add(ResolveArgument(exprArgument));
                }
                try
                {
                    return fn.Call(fn.ResolveArguments(arguments));
                }
                catch(ReturnException ret)
                {
                    throw ret;
                }
                catch (BreakException brk)
                {
                    throw brk;
                }
                catch (ContinueException cont)
                {
                    throw cont;
                }
                catch (ExitException ee)
                {
                    throw ee;
                }
                catch (Exception e)
                {
                    throw new Exception($"Error in Call [symbol:{exprCall.Symbol}] location: {exprCall.Loc}\n\t{e.ToString()}");
                }
            } else
            {
                throw new Exception($"error converting object {callable} to native callable type. location: {exprCall.Loc}");
            }
        }

        private Argument ResolveArgument(ExprArgument exprArgument)
        {
            Argument argument = new Argument();
            argument.Alias = exprArgument.ParameterAlias;
            argument.Position = exprArgument.Position;
            argument.Value = Accept(exprArgument.Value);
            return argument;
        }

        public virtual QualifiedObject Accept(ExprFnDeclaration exprFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (StmtParameter stmtParameter in exprFnDeclaration.Parameters)
            {
                resolvedParameters.Add(ResolveParameter(stmtParameter));
            }
            Function fn = new UserDefinedFunction(_environment, exprFnDeclaration, resolvedParameters);
            return  QualifiedObjectBuilder.BuildFunction(fn);
        }

        public virtual QualifiedObject Accept(ExprNullableSwitch exprNullableSwitch)
        {
            QualifiedObject lhs = Accept(exprNullableSwitch.Lhs);
            if (lhs.Type.Is(Enums.DataTypeEnum.Empty))
            {
                return Accept(exprNullableSwitch.Rhs);
            }
            return lhs;
        }

        public virtual QualifiedObject Accept(ExprGetArgument exprGetArgument)
        {
            QualifiedObject lhs = Accept(exprGetArgument.Lhs);
            BoundFn fn = TypeMediatorService.ToNativeType<BoundFn>(lhs);
            if (exprGetArgument.Strict || fn.BoundArguments.ContainsKey(exprGetArgument.Identifier))
            {
                return fn.BoundArguments.Get(exprGetArgument.Identifier);
            }
            return QualifiedObjectBuilder.BuildEmptyValue();

        }

        public virtual QualifiedObject Accept(ExprIdentifier exprIdentifier)
        {
            return SymbolResolverHelper.Resolve(_environment, exprIdentifier.Symbol);
        }

        public virtual QualifiedObject Accept(ExprLiteral exprLiteral)
        {
            return exprLiteral.Value;
        }
    }
}
