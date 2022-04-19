using Microsoft.VisualStudio.Text;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.IDE._Parser.Helpers;
using Mochj.IDE._Parser.Models;
using Mochj.IDE._Parser.Models.Expressions;
using Mochj.IDE._Parser.Models.Statements;
using Mochj.IDE._Tokenizer.Models;
using Mochj.Models;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser
{
    public class InformativeParser2 : ParsingHelper
    {

        private NumberFormatInfo DefaultNumberFormat = new NumberFormatInfo { NegativeSign = "-" };
        public IEnumerable<RangedStatement> Parse(IEnumerable<RangedToken> tokens)
        {
            init(tokens);
            IList<RangedStatement> statements = new List<RangedStatement>();
            while (!atEnd() && !match(TokenTypes.EOF))
            {
                var stmt = parseStatement();
                if (stmt != null)
                {
                    statements.Add(stmt);
                }
            }
            return statements;
        }


        private RangedStatement parseStatement()
        {
            try
            {
                if (match(TokenTypes.At))
                {
                    if (match(TokenTypes.Set))
                    {
                        return parseSet();
                    }
                    if (match(TokenTypes.Defn))
                    {
                        return parseFunctionDeclaration();
                    }
                    if (match(TokenTypes.Namespace))
                    {
                        return parseNamespace();
                    }
                    if (match(TokenTypes.Load))
                    {
                        return parseLoad();
                    }
                    if (match(TokenTypes.Entry))
                    {
                        return parseEntry();
                    }
                    throw new Exception($"unexpected token where RangedStatement expected: {current()}");
                }

                RangedStatement stmt = parseExpressionStatement();
                if (stmt is StmtExpression stmtExpr && stmtExpr.Expression is ExprFnDeclaration exprFn)
                {
                    return new StmtFnDeclaration(exprFn.Loc)
                    {
                        Label = exprFn.Label,
                        Name = exprFn.Name,
                        Parameters = exprFn.Parameters,
                        ReturnType = exprFn.ReturnType,
                        Statements = exprFn.Statements,
                        TextRange = exprFn.TextRange,
                    };
                }
                return stmt;
            } catch (Exception)
            {
                if (!atEnd()) advance();
                return null;
            }
        }
        private RangedStatement parseSet()
        {
            int stmtStart = previous().TextRange.Start;

            StmtSet stmt = new StmtSet(previous().Token.Loc);
            stmt.Identifier = consume(TokenTypes.TTWord, "expect identifier in 'set'");
            stmt.Value = parseExpression();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'set'");
            int stmtEnd = previous().TextRange.End;
            stmt.TextRange = new Span(stmtStart, stmtEnd);

            return stmt;
        }

        private RangedStatement parseNamespace()
        {
            int stmtStart = previous().TextRange.Start;

            StmtNamespace stmt = new StmtNamespace(previous().Token.Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'namespace'");
            int stmtEnd = previous().TextRange.End;
            stmt.TextRange = new Span(stmtStart, stmtEnd);

            return stmt;
        }

        private RangedStatement parseLoad()
        {
            int stmtStart = previous().TextRange.Start;

            StmtLoad stmt = new StmtLoad(previous().Token.Loc);
            stmt.Path = consume(TokenTypes.TTString, "expect filepath in 'load'");
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'load'");
            int stmtEnd = previous().TextRange.End;
            stmt.TextRange = new Span(stmtStart, stmtEnd);

            return stmt;
        }

        private RangedStatement parseEntry()
        {
            int stmtStart = previous().TextRange.Start;

            StmtEntry stmt = new StmtEntry(previous().Token.Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'entry'");
            int stmtEnd = previous().TextRange.End;
            stmt.TextRange = new Span(stmtStart, stmtEnd);

            return stmt;
        }


        private Symbol parseSymbol()
        {
            List<RangedToken> names = new List<RangedToken>();
            Location location;
            do
            {
                RangedToken token = consume(TokenTypes.TTWord, "expect name symbol");
                location = token.Token.Loc;
                names.Add(token);
            } while (match(TokenTypes.Dot));
            return new Symbol { Loc = location, Names = names };
        }

        private RangedStatement parseFunctionDeclaration()
        {
            int stmtStart = previous().TextRange.Start;

            StmtParameter fnNameRetType = parseParameter(0, false);
            StmtFnDeclaration stmtFnDeclaration = new StmtFnDeclaration(fnNameRetType.Loc);
            stmtFnDeclaration.Name = fnNameRetType.Alias;
            stmtFnDeclaration.ReturnType = fnNameRetType.DataType;
            stmtFnDeclaration.Parameters = new List<StmtParameter>();
            int paramCount = 0;
            while (match(TokenTypes.LBracket))
            {
                stmtFnDeclaration.Parameters.Add(parseParameter(paramCount, true));
                consume(TokenTypes.RBracket, "expect enclosing ']' for parameter");
                paramCount++;
            }
            stmtFnDeclaration.Statements = new List<RangedStatement>();
            while (!match(TokenTypes.SemiColon))
            {
                stmtFnDeclaration.Statements.Add(parseStatement());
            }
            int stmtEnd = previous().TextRange.End;
            stmtFnDeclaration.TextRange = new Span(stmtStart, stmtEnd);

            return stmtFnDeclaration;
        }

        private StmtParameter parseParameter(int paramCount, bool allowDefaultValue = true)
        {
            StmtParameter stmtParameter = new StmtParameter();
            RangedToken alias = consume(TokenTypes.TTWord, "expect parameter name");
            stmtParameter.Loc = alias.Token.Loc;
            stmtParameter.Alias = alias;
            stmtParameter.Position = paramCount;
            consume(TokenTypes.Colon, "expect :TypeName in parameter declaration");
            stmtParameter.DataType = parseDataType();
            if (allowDefaultValue && match(TokenTypes.Equal))
            {
                stmtParameter.Default = parseExpression();
            }
            return stmtParameter;
        }

        private DataType parseDataType()
        {
            if (match(TokenTypes.Number))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Number };
            }
            if (match(TokenTypes.Boolean))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Boolean };
            }
            if (match(TokenTypes.Fn))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Fn };
            }
            if (match(TokenTypes.String))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.String };
            }
            if (match(TokenTypes.Empty))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Empty };
            }
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Any };
            }
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.Any };
            }
            if (match(TokenTypes.TypeInfo))
            {
                return new DataType { TypeId = Mochj.Enums.DataTypeEnum.TypeInfo };
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = Mochj.Enums.DataTypeEnum.NativeList };
                consume(TokenTypes.LCarat, "expect List<contained-typename> ");
                dt.ContainedType = parseDataType();
                consume(TokenTypes.RCarat, "expect enclosing '>'");
                return dt;
            }
            throw new Exception($"unrecognized type: {current()}");
        }
        private RangedStatement parseExpressionStatement()
        {
            int stmtStart = previous().TextRange.Start;

            StmtExpression stmtExpression = new StmtExpression(current().Token.Loc);
            stmtExpression.Expression = parseExpression();
            int stmtEnd = previous().TextRange.End;
            stmtExpression.TextRange = new Span(stmtStart, stmtEnd);

            return stmtExpression;
        }

        private RangedExpression parseExpression()
        {
            return parseBinary();
        }

        private RangedExpression parseBinary()
        {
            int stmtStart = previous().TextRange.Start;

            RangedExpression expr = parseCall();
            if (match(TokenTypes.DoubleQuestionMark))
            {
                expr = new ExprNullableSwitch(previous().Token.Loc, expr, parseBinary());
            }
            else while (match(TokenTypes.DoubleDot))
                {
                    var exprGetArg = new ExprGetArgument(previous().Token.Loc, expr);
                    exprGetArg.Identifier = consume(TokenTypes.TTWord).Token.Lexeme;
                    expr = exprGetArg;
                }
            int stmtEnd = previous().TextRange.End;
            expr.TextRange = new Span(stmtStart, stmtEnd);

            return expr;
        }

        private RangedExpression parseLiteralFunctionDeclaration()
        {
            int stmtStart = previous().TextRange.Start;
            ExprFnDeclaration exprFnDeclaration = new ExprFnDeclaration(previous().Token.Loc);
            if (match(current(), TokenTypes.TTWord))
            {
                StmtParameter fnNameRetType = parseParameter(0, false);
                exprFnDeclaration.Name = fnNameRetType.Alias;
                exprFnDeclaration.ReturnType = fnNameRetType.DataType;
            }
            else
            {
                // It is lambda function
                exprFnDeclaration.Name = new RangedToken {Token = new Token(TokenTypes.TTWord, "[lambda]",0,0), TextRange = new Span(0,0) };
                exprFnDeclaration.ReturnType = new DataType { TypeId = Mochj.Enums.DataTypeEnum.Empty };
            }

            exprFnDeclaration.Parameters = new List<StmtParameter>();
            int paramCount = 0;
            while (match(TokenTypes.LBracket))
            {
                exprFnDeclaration.Parameters.Add(parseParameter(paramCount, true));
                consume(TokenTypes.RBracket, "expect enclosing ']' for parameter");
                paramCount++;
            }
            exprFnDeclaration.Statements = new List<RangedStatement>();
            while (!match(TokenTypes.RParen))
            {
                exprFnDeclaration.Statements.Add(parseStatement());
            }
            int stmtEnd = previous().TextRange.End;
            exprFnDeclaration.TextRange = new Span(stmtStart, stmtEnd);
            return exprFnDeclaration;
        }


        private RangedExpression parseCall()
        {
            int stmtStart = previous().TextRange.Start;
            if (match(TokenTypes.LParen))
            {
                if (match(TokenTypes.Defn))
                {
                    return parseLiteralFunctionDeclaration();
                }
                Symbol sym = parseSymbol();
                ExprCall exprCall = new ExprCall(previous().Token.Loc);
                exprCall.Symbol = sym;
                exprCall.Arguments = new List<ExprArgument>();

                bool requireExplicitTypes = false;
                int argCount = 0;
                while (!match(TokenTypes.RParen))
                {
                    exprCall.Arguments.Add(parseArgument(argCount, ref requireExplicitTypes));
                    argCount++;
                }
                int stmtEnd = previous().TextRange.End;
                exprCall.TextRange = new Span(stmtStart, stmtEnd);

                return exprCall;
            }
            return parsePrimary();
        }

        private ExprArgument parseArgument(int argCount, ref bool wasExplicitlyNamed)
        {
            ExprArgument exprArgument = new ExprArgument(previous().Token.Loc);
            if (wasExplicitlyNamed)
            {
                consume(TokenTypes.DoubleDash, "expect explicit parameter name '--<param-name>'");
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Token.Lexeme;
                wasExplicitlyNamed = true;
            }
            else if (match(TokenTypes.DoubleDash))
            {
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Token.Lexeme;
                wasExplicitlyNamed = true;
            }
            else
            {
                exprArgument.Position = argCount;
            }
            exprArgument.Value = parseExpression();
            return exprArgument;
        }

        private RangedExpression parsePrimary()
        {
            ExprLiteral exprLiteral = new ExprLiteral(current().Token.Loc);
            if (match(TokenTypes.LiteralFalse))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildBoolean(false);
                return exprLiteral;
            }
            if (match(TokenTypes.LiteralTrue))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildBoolean(true);
                return exprLiteral;
            }
            if (match(TokenTypes.TTInteger))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(int.Parse(previous().Token.Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTUnsignedInteger))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(uint.Parse(previous().Token.Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTFloat))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(float.Parse(previous().Token.Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTDouble))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(double.Parse(previous().Token.Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTString))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildString(previous().Token.Lexeme);
                return exprLiteral;
            }
            if (match(current(), TokenTypes.Dash) &&
                (peekMatch(1, TokenTypes.TTUnsignedInteger)
                || peekMatch(1, TokenTypes.TTInteger)
                || peekMatch(1, TokenTypes.TTFloat)
                || peekMatch(1, TokenTypes.TTDouble)))
            {
                advance();
                if (match(TokenTypes.TTInteger))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(int.Parse("-" + previous().Token.Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTUnsignedInteger))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(uint.Parse("-" + previous().Token.Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTFloat))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(float.Parse("-" + previous().Token.Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTDouble))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(double.Parse("-" + previous().Token.Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                throw new Exception($"unexpected token while parsing negative {current()}");
            }
            if (match(current(), TokenTypes.TTWord))
            {
                ExprIdentifier exprIdentifier = new ExprIdentifier(previous().Token.Loc);
                exprIdentifier.Symbol = parseSymbol();
                return exprIdentifier;
            }
            if (match(TokenTypes.Number))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<double>();
                return exprLiteral;
            }
            if (match(TokenTypes.Boolean))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<bool>();
                return exprLiteral;
            }
            if (match(TokenTypes.Fn))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<Function>();
                return exprLiteral;
            }
            if (match(TokenTypes.String))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<string>();
                return exprLiteral;
            }
            if (match(TokenTypes.TypeInfo))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<DataType>();
                return exprLiteral;
            }
            if (match(TokenTypes.Empty))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo(new DataType { TypeId = Mochj.Enums.DataTypeEnum.Empty });
                return exprLiteral;
            }
            if (match(TokenTypes.Any))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<object>();
                return exprLiteral;
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = Mochj.Enums.DataTypeEnum.NativeList };
                consume(TokenTypes.LCarat, "expect List<contained-typename> ");
                dt.ContainedType = parseDataType();
                consume(TokenTypes.RCarat, "expect enclosing '>'");
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo(dt);
                return exprLiteral;
            }
            throw new Exception($"unexpected token in primary {current()}");
        }
    }
}
