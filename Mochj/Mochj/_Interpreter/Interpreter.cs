using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj.Builders;
using Mochj.Models;
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
            }else
            {
                _environment = environment;
            }
        }

        public Function GetEntryPoint()
        {
            if (_entryPoint == null)
            {
                throw new Exception("entry point not defined");
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

        public void Accept(IEnumerable<Statement> statements)
        {
            foreach(Statement statement in statements)
            {
                Accept(statement);
            }
        }

        internal void Accept(Statement statement)
        {
            statement.Visit(this);
        }

        internal void Accept(StmtNamespace stmtNamespace)
        {
            _environment = SymbolResolverHelper.ResolveToNamespace(_environment.Top(), stmtNamespace.Symbol);
        }

        internal void Accept(StmtLoad stmtLoad)
        {
            string path = stmtLoad.Path;
            if (!File.Exists(path))
            {
                path = LoadFileHelper.SwitchPathToExecutableHome(path);
                if (!File.Exists(path))
                {
                    throw new Exception($"unable to open {stmtLoad.Path}");
                }
            }
            if (Path.GetExtension(stmtLoad.Path) == ".dll")
            {
                LoadFileHelper.LoadFromAssembly(_environment.Top(), path);
            }
            else
            {
                LoadFileHelper.LoadFromRawCode(_environment.Top(), path);
            }
        }

        internal void Accept(StmtEntry stmtEntry)
        {
            _entryPoint = stmtEntry.Symbol;
        }

        internal void Accept(StmtSet stmtSet)
        {
            if (SymbolResolverHelper.Resolvable(_environment, stmtSet.Identifier))
            {
                StorageHelper.AssignStrict(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            } else
            {
                StorageHelper.Define(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
        }

        internal void Accept(StmtFnDeclaration stmtFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (StmtParameter stmtParameter in stmtFnDeclaration.Parameters)
            {
                resolvedParameters.Add(ResolveParameter(stmtParameter));
            }
            Models.Fn.Function fn = new UserDefinedFunction(_environment, stmtFnDeclaration, resolvedParameters);
            StorageHelper.Define(_environment, fn.Name, QualifiedObjectBuilder.BuildFunction(fn));
        }

        private Parameter ResolveParameter(StmtParameter stmtParameter)
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

        internal void Accept(StmtExpression stmtExpression)
        {
            Accept(stmtExpression.Expression);
        }

        internal QualifiedObject Accept(Expression expression)
        {
            return expression.Visit(this);
        }
        internal QualifiedObject Accept(ExprCall exprCall)
        {
            QualifiedObject callable = SymbolResolverHelper.Resolve(_environment, exprCall.Symbol);
            if (!callable.Type.Is(Enums.DataTypeEnum.Fn))
            {
                throw new Exception($"[symbol:{exprCall.Symbol}] expected object of type '{Enums.DataTypeEnum.Fn}' but got '{callable.Type}'");
            }
            if (callable.Object is Models.Fn.Function fn)
            {
                IList<Argument> arguments = new List<Argument>();
                foreach(ExprArgument exprArgument in exprCall.Arguments)
                {
                    arguments.Add(ResolveArgument(exprArgument));
                }
                return fn.Call(fn.ResolveArguments(arguments));
            } else
            {
                throw new Exception($"error converting object {callable} to native callable type");
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

        internal QualifiedObject Accept(ExprFnDeclaration exprFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (StmtParameter stmtParameter in exprFnDeclaration.Parameters)
            {
                resolvedParameters.Add(ResolveParameter(stmtParameter));
            }
            Models.Fn.Function fn = new UserDefinedFunction(_environment, exprFnDeclaration, resolvedParameters);
            return  QualifiedObjectBuilder.BuildFunction(fn);
        }

        internal QualifiedObject Accept(ExprIdentifier exprIdentifier)
        {
            return SymbolResolverHelper.Resolve(_environment, exprIdentifier.Symbol);
        }

        internal QualifiedObject Accept(ExprLiteral exprLiteral)
        {
            return exprLiteral.Value;
        }
    }
}
