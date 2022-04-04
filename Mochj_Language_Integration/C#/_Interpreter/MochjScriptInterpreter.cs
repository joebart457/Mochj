using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using Mochj.Services;
using MochjLanguage._Interpreter.Helpers;
using MochjLanguage.Service;
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

        private List<string> _currentFnParameterSymbols = new List<string>();

        public List<(Mochj._Storage.Environment, List<Statement>)> ReInterpret { get; set; } = new List<(Mochj._Storage.Environment, List<Statement>)>();

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
            MochjScriptingService.FnTokens = new List<Token>();
            MochjScriptingService.IdentifierTokens = new List<Token>();
        }


        public override void Accept(IEnumerable<Statement> statements)
        {
            ReInterpret = new List<(Mochj._Storage.Environment, List<Statement>)>();
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
            } catch (Exception e)
            {
                var x = e;
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
                StorageHelper.Define(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
            else
            {
                StorageHelper.Define(_environment, stmtSet.Identifier, Accept(stmtSet.Value));
            }
        }

        public override void Accept(StmtFnDeclaration stmtFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            _currentFnParameterSymbols = new List<string>();
            foreach (StmtParameter stmtParameter in stmtFnDeclaration.Parameters)
            {
                var param = ResolveParameter(stmtParameter);
                resolvedParameters.Add(param);
                _currentFnParameterSymbols.Add(param.Alias);
            }
            Function fn = new UserDefinedFunction(_environment, stmtFnDeclaration, resolvedParameters);
            StorageHelper.Define(_environment, fn.Name, QualifiedObjectBuilder.BuildFunction(fn));
            var previous = _environment;
            _environment = new Mochj._Storage.Environment(previous);
            try
            {
                foreach(var param in resolvedParameters)
                {
                    var obj = DefaultObjectBuilder.BuildDefault(param.Type);
                    obj.Object = "<param>";
                    _environment.Define(param.Alias, obj);
                }
                ReInterpret.Add((_environment, stmtFnDeclaration.Statements.ToList()));
            } catch(Exception e)
            {

            }
            _environment = previous;
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

                MochjScriptingService.FnTokens.Add(new Token("_Interpreted_Fn", exprCall.Symbol.Names.Last(), exprCall.Loc.Y, exprCall.Loc.X));

                foreach(var arg in exprCall.Arguments)
                {
                    Accept(arg.Value);
                }


                return DefaultObjectBuilder.BuildDefault(fn.ReturnType);
            }
            catch (Exception e)
            {
                return DefaultObjectBuilder.BuildUnknown();
            }
        }

        public override QualifiedObject Accept(ExprFnDeclaration exprFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            _currentFnParameterSymbols = new List<string>();
            foreach (StmtParameter stmtParameter in exprFnDeclaration.Parameters)
            {
                var param = ResolveParameter(stmtParameter);
                resolvedParameters.Add(param);
                _currentFnParameterSymbols.Add(param.Alias);
            }
            Function fn = new UserDefinedFunction(_environment, exprFnDeclaration, resolvedParameters);
            var previous = _environment;
            _environment = new Mochj._Storage.Environment(previous);
            try
            {
                foreach (var param in resolvedParameters)
                {
                    var obj = DefaultObjectBuilder.BuildDefault(param.Type);
                    obj.Object = "<param>";
                    _environment.Define(param.Alias, obj);
                }
                ReInterpret.Add((_environment, exprFnDeclaration.Statements.ToList()));

            }
            catch (Exception e)
            {
                
            }
            _environment = previous;
            return QualifiedObjectBuilder.BuildFunction(fn);
        }

        public override QualifiedObject Accept(ExprIdentifier exprIdentifier)
        {
            try
            {
                var obj = SymbolResolverHelper.Resolve(_environment, exprIdentifier.Symbol);

                string type = "_Interpreted_Id";
                if (obj.Equals("<param>"))
                {
                    type = "_Interpreted_Param";
                }

                MochjScriptingService.IdentifierTokens.Add(new Token(type, exprIdentifier.Symbol.Names.Last(), exprIdentifier.Loc.Y, exprIdentifier.Loc.X));

                return DefaultObjectBuilder.BuildDefault(obj.Type);
            } catch (Exception e)
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
