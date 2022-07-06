using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj.Builders;
using Mochj.IDE._Interpreter.Models;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj.IDE._Parser.Models.Statements;
using Mochj.IDE.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = Mochj._Storage.Environment;


namespace Mochj.IDE._Interpreter
{
    public class InformedInterpreter
    {
        private class PassData
        {
            public List<RangedStatement> Statements { get; set; }
            public Environment Environment { get; set; }
            public RangedStatementWithEnv ParentSpan { get; set; } = null;
        }

        private Symbol _entry;
        private Environment _environment;
        private const int PassLimit = 5;
        private int _passIndex = 0;
        private List<PassData> _nextPassData { get; set; } = new List<PassData>();
        private List<PassData> _lastPassData { get; set; } = new List<PassData>();

        private RangedStatementWithEnv _parentSpan = null;
        private RangedStatementWithEnv _currentSpan = null;
        public InformedInterpreter(Environment environment = null)
        {
            _environment = environment == null ? new Environment() : environment;
        }
        public ExecutionInfo Interpret(List<RangedStatement> statements)
        {
            var span = new RangedStatementWithEnv { Statement = null, Environment = _environment};
            RegisterForNextPass(statements, _environment, span);
            while(_passIndex != PassLimit)
            {
                DoNextPass();
            }
            return new ExecutionInfo { Environment = _environment, Spans = span.ChildSpans };
        }

        private void RegisterForNextPass(List<RangedStatement> stmts, Environment environment, RangedStatementWithEnv span)
        {
            _nextPassData.Add(new PassData { Statements = stmts, Environment = environment, ParentSpan = span });
        }

        private void RegisterForLastPass(List<RangedStatement> stmts, Environment environment, RangedStatementWithEnv span)
        {
            _lastPassData.Add(new PassData { Statements = stmts, Environment = environment, ParentSpan = span });
        }

        private void DoNextPass()
        {
            if (_passIndex == PassLimit) return;
            _passIndex++;
            if (_passIndex == PassLimit || !_nextPassData.Any()) DoLastPass();

            var nextPassData = _nextPassData;
            _nextPassData = new List<PassData>();
            foreach(var passData in nextPassData)
            {
                DoPass(passData);
            }
            
        }

        private void DoLastPass()
        {
            _passIndex = PassLimit;
            foreach(var passData in _lastPassData)
            {
                DoPass(passData);
            }
        }

        private void DoPass(PassData data)
        {
            var previous = _environment;
            try
            {
                _environment = data.Environment;
                if (data.ParentSpan != null)
                {
                    _parentSpan = data.ParentSpan;
                } else
                {
                    throw new Exception("Parent span not specified");
                }
                Accept(data.Statements);
            }
            catch (Exception) { }
            _environment = previous;
        }

        public void Accept(List<RangedStatement> stmts)
        {
            foreach(var stmt in stmts)
            {
                if (stmt != null) Accept(stmt);
            }
        }

        public void Accept(RangedStatement stmt)
        {
            _currentSpan = new RangedStatementWithEnv
            {
                Statement = stmt,
                Environment = _environment
            };
            stmt.Visit(this);
            _parentSpan.ChildSpans.Add(_currentSpan);
        }

        public void Accept(StmtNamespace stmtNamespace)
        {
            Symbol mochjSymbol = new Symbol
            {
                Location = stmtNamespace.Symbol.Loc,
                Names = stmtNamespace.Symbol.Names.Select(t => t.Token.Lexeme).ToList()
            };
            try
            {
                _environment = SymbolResolverHelper.ResolveToNamespace(_environment.Top(), mochjSymbol);
                stmtNamespace.Symbol.Names.ForEach(n => n.Classifier = Enums.TokenClassifierEnum.Namespace);
            }
            catch (Exception)
            {

            }
           
        }

        public void Accept(StmtLoad stmtLoad)
        {
            string path = stmtLoad.Path.Token.Lexeme;
            if (!File.Exists(path))
            {
                path = LoadFileHelper.SwitchPathToExecutableHome(path);
                if (!File.Exists(path))
                {
                    stmtLoad.Path.Classifier = Enums.TokenClassifierEnum.Warning;
                    stmtLoad.Path.Message = "Filepath does not exist";
                }
            }
            try
            {
                LoadFileHelper.LoadFile(_environment.Top(), path);
            }
            catch (Exception) { }
        }

        public void Accept(StmtEntry stmtEntry)
        {
           // _entry = stmtEntry.Symbol;
        }

        public void Accept(StmtSet stmtSet)
        {
            try
            {
                if (SymbolResolverHelper.Resolvable(_environment, stmtSet.Identifier.Token.Lexeme))
                {
                    StorageHelper.AssignStrict(_environment, stmtSet.Identifier.Token.Lexeme, Accept(stmtSet.Value));
                }
                else
                {
                    StorageHelper.Define(_environment, stmtSet.Identifier.Token.Lexeme, Accept(stmtSet.Value));
                }
                stmtSet.Identifier.Classifier = Enums.TokenClassifierEnum.Identifier_1;
            }
            catch (Exception e)
            {
                stmtSet.Identifier.Message = e.Message;
                stmtSet.Identifier.Classifier = Enums.TokenClassifierEnum.Error_1;
            }
        }

        public void Accept(StmtFnDeclaration stmtFnDeclaration)
        {
            IList<Parameter> resolvedParameters = new List<Parameter>();
            foreach (var stmtParameter in stmtFnDeclaration.Parameters)
            {
                var param = ResolveParameter(stmtParameter);
                stmtParameter.Alias.Classifier = Enums.TokenClassifierEnum.Identifier_1;
                resolvedParameters.Add(param);
            }
            Function fn = new UserDefinedFunction(_environment,
                new Mochj._Parser.Models.Expressions.ExprFnDeclaration(stmtFnDeclaration.Loc)
                {
                    Name = stmtFnDeclaration.Name.Token.Lexeme,
                    ReturnType = stmtFnDeclaration.ReturnType,
                    Parameters = stmtFnDeclaration.Parameters.Select(p => new Mochj._Parser.Models.Statements.StmtParameter { Alias = p.Alias.Token.Lexeme, DataType = p.DataType, Loc = p.Loc, Position = p.Position }).ToList(),
                },
            resolvedParameters);
            StorageHelper.Define(_environment, fn.Name, QualifiedObjectBuilder.BuildFunction(fn));
            stmtFnDeclaration.Name.Classifier = Enums.TokenClassifierEnum.Function;
            var previous = _environment;
            _environment = new Environment(previous);
            try
            {
                RegisterForNextPass(stmtFnDeclaration.Statements.ToList(), _environment, _currentSpan);

                foreach (var param in resolvedParameters)
                {
                    var obj = DefaultObjectBuilder.BuildDefault(param.Type);
                    _environment.Define(param.Alias, obj);
                }
            }
            catch (Exception e)
            {

            }
            _environment = previous;
        }

        public Parameter ResolveParameter(StmtParameter stmtParameter)
        {
            Parameter parameter = new Parameter();
            parameter.Alias = stmtParameter.Alias.Token.Lexeme;
            parameter.Type = stmtParameter.DataType;
            parameter.Position = stmtParameter.Position;
            //if (stmtParameter.Default != null)
            //{
            //    parameter.Default = TypeHelper.CheckType(Accept(stmtParameter.Default), parameter.Type);
            //}
            return parameter;
        }

        public void Accept(StmtExpression stmtExpression)
        {
            Accept(stmtExpression.Expression);
        }


        public virtual QualifiedObject Accept(RangedExpression expression)
        {
            return expression.Visit(this);
        }
        public virtual QualifiedObject Accept(ExprCall exprCall)
        {
            try
            {
                foreach (var arg in exprCall.Arguments)
                {
                    ResolveArgument(arg);
                }

                Symbol mochjSymbol = new Symbol
                {
                    Location = exprCall.Symbol.Loc,
                    Names = exprCall.Symbol.Names.Select(t => t.Token.Lexeme).ToList()
                };
                var obj = SymbolResolverHelper.Resolve(_environment, mochjSymbol);

                exprCall.Symbol.Names.ForEach(t => t.Classifier = Enums.TokenClassifierEnum.Namespace);
                exprCall.Symbol.Names.Last().Classifier = Enums.TokenClassifierEnum.Function;

                Function fn = TypeMediatorService.ToNativeType<Function>(obj);



                return DefaultObjectBuilder.BuildDefault(fn.ReturnType);
            }
            catch (Exception e)
            {
                var tok = exprCall.Symbol.Names.LastOrDefault();
                if (tok != null)
                {
                    tok.Classifier = Enums.TokenClassifierEnum.Error_1;
                    tok.Message = "Unable to resolve symbol";
                }
                return DefaultObjectBuilder.BuildUnknown();
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
            foreach (var stmtParameter in exprFnDeclaration.Parameters)
            {
                var param = ResolveParameter(stmtParameter);
                stmtParameter.Alias.Classifier = Enums.TokenClassifierEnum.Identifier_1;
                resolvedParameters.Add(param);
            }
            Function fn = new UserDefinedFunction(_environment, 
                new Mochj._Parser.Models.Expressions.ExprFnDeclaration(exprFnDeclaration.Loc) {
                    Name = exprFnDeclaration.Name.Token.Lexeme,
                    ReturnType = exprFnDeclaration.ReturnType,
                    Parameters = exprFnDeclaration.Parameters.Select(p => new Mochj._Parser.Models.Statements.StmtParameter { Alias = p.Alias.Token.Lexeme, DataType = p.DataType, Loc = p.Loc, Position = p.Position}).ToList(),
            },
            resolvedParameters);
            exprFnDeclaration.Name.Classifier = Enums.TokenClassifierEnum.Function;
            var previous = _environment;
            _environment = new Environment(previous);
            try
            {
                RegisterForNextPass(exprFnDeclaration.Statements.ToList(), _environment, _currentSpan);

                foreach (var param in resolvedParameters)
                {
                    var obj = DefaultObjectBuilder.BuildDefault(param.Type);
                    _environment.Define(param.Alias, obj);
                }
            }
            catch (Exception e)
            {

            }
            _environment = previous;
            return QualifiedObjectBuilder.BuildFunction(fn);
        }

        public virtual QualifiedObject Accept(ExprNullableSwitch exprNullableSwitch)
        {
            QualifiedObject lhs = Accept(exprNullableSwitch.Lhs);
            QualifiedObject rhs = Accept(exprNullableSwitch.Rhs);
            if (lhs.Type.Is(Mochj.Enums.DataTypeEnum.Empty))
            {
                return rhs;
            }
            return lhs;
        }

        public virtual QualifiedObject Accept(ExprGetArgument exprGetArgument)
        {
            try
            {
                QualifiedObject lhs = Accept(exprGetArgument.Lhs);
                BoundFn fn = TypeMediatorService.ToNativeType<BoundFn>(lhs);
                return fn.BoundArguments.Get(exprGetArgument.Identifier);
            } catch(Exception e)
            {
                return DefaultObjectBuilder.BuildEmpty();
            }
        }

        public virtual QualifiedObject Accept(ExprIdentifier exprIdentifier)
        {
            try
            {
                Symbol mochjSymbol = new Symbol
                {
                    Location = exprIdentifier.Symbol.Loc,
                    Names = exprIdentifier.Symbol.Names.Select(t => t.Token.Lexeme).ToList()
                };
                var obj = SymbolResolverHelper.Resolve(_environment, mochjSymbol);

                exprIdentifier.Symbol.Names.ForEach(t => t.Classifier = Enums.TokenClassifierEnum.Namespace);
                exprIdentifier.Symbol.Names.Last().Classifier = Enums.TokenClassifierEnum.Identifier_1;

                return DefaultObjectBuilder.BuildDefault(obj.Type);
            }
            catch (Exception e)
            {
                exprIdentifier.Symbol.Names.Last().Classifier = Enums.TokenClassifierEnum.Error_1;
                exprIdentifier.Symbol.Names.Last().Message = "Identifier is undeclared";
                return DefaultObjectBuilder.BuildUnknown();
            }
        }

        public virtual QualifiedObject Accept(ExprLiteral exprLiteral)
        {
            return exprLiteral.Value;
        }

    }
}
