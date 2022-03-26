using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using Mochj.Services;
using MochjLanguage._Interpreter.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MochjLanguage._Interpreter
{
    class MochjScriptInterpreter: Mochj._Interpreter.Interpreter
    {
        private Mochj._Storage.Environment _environment;
        private Symbol _entryPoint;
        public MochjScriptInterpreter(Mochj._Storage.Environment environment)
            :base(environment)
        {
            if (environment == null)
            {
                _environment = new Mochj._Storage.Environment(null);
            }
            else
            {
                _environment = environment;
            }
        }


        public override void Accept(IEnumerable<Statement> statements)
        {
            foreach (Statement statement in statements)
            {
                Accept(statement);
            }
        }

        public override void Accept(Statement statement)
        {
            try
            {
                statement.Visit(this);
            } catch (Exception)
            {

            }
        }


        public override void Accept(StmtNamespace stmtNamespace)
        {
            _environment = SymbolResolverHelper.ResolveToNamespace(_environment.Top(), stmtNamespace.Symbol);
        }

        public override void Accept(StmtLoad stmtLoad)
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

        public override void Accept(StmtEntry stmtEntry)
        {
            _entryPoint = stmtEntry.Symbol;
        }

        public override void Accept(StmtSet stmtSet)
        {
            if (SymbolResolverHelper.Resolvable(_environment, stmtSet.Identifier))
            {
                StorageHelper.AssignStrict(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
            else
            {
                StorageHelper.Define(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
        }

        public override void Accept(StmtFnDeclaration stmtFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (StmtParameter stmtParameter in stmtFnDeclaration.Parameters)
            {
                resolvedParameters.Add(ResolveParameter(stmtParameter));
            }
            Function fn = new UserDefinedFunction(_environment, stmtFnDeclaration, resolvedParameters);
            StorageHelper.Define(_environment, fn.Name, QualifiedObjectBuilder.BuildFunction(fn));
        }

        public override Parameter ResolveParameter(StmtParameter stmtParameter)
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

        public override void Accept(StmtExpression stmtExpression)
        {
            Accept(stmtExpression.Expression);
        }

        public override QualifiedObject Accept(Expression expression)
        {
            return expression.Visit(this);
        }
        public override QualifiedObject Accept(ExprCall exprCall)
        {
            try
            {
                QualifiedObject callable = SymbolResolverHelper.Resolve(_environment, exprCall.Symbol);
                Function fn = TypeMediatorService.ToNativeType<Function>(callable);
                return DefaultObjectBuilder.BuildDefault(fn.ReturnType);
            }
            catch (Exception)
            {
                return DefaultObjectBuilder.BuildUnknown();
            }
        }

        public override QualifiedObject Accept(ExprFnDeclaration exprFnDeclaration)
        {
            return DefaultObjectBuilder.BuildFn();
        }

        public override QualifiedObject Accept(ExprIdentifier exprIdentifier)
        {
            try
            {
                var obj = SymbolResolverHelper.Resolve(_environment, exprIdentifier.Symbol);
                return DefaultObjectBuilder.BuildDefault(obj.Type);
            } catch (Exception)
            {
                return DefaultObjectBuilder.BuildUnknown();
            }
        }

        public override QualifiedObject Accept(ExprLiteral exprLiteral)
        {
            return exprLiteral.Value;
        }
    }
}
