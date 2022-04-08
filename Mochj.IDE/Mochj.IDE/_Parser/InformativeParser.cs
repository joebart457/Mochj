using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Enums;
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
using System.Windows.Documents;

namespace Mochj.IDE._Parser
{
    public class InformativeParser : ParsingHelper
    {

        private NumberFormatInfo DefaultNumberFormat = new NumberFormatInfo { NegativeSign = "-" };
        public IEnumerable<RangedStatement> Parse(IEnumerable<RangedToken> tokens, out RangedToken invalidRangedToken)
        {
            init(tokens);
            IList<RangedStatement> statements = new List<RangedStatement>();
            while (!atEnd() && !match(TokenTypes.EOF))
            {
                if (match(TokenTypes.LParen))
                {

                    var stmt = parseStatement();
                    if (stmt != null) statements.Add(stmt);

                    continue;
                }
                invalidRangedToken = current();
                return statements;
            }
            invalidRangedToken = null;
            return statements;
        }


        private RangedStatement parseStatement()
        {
            try
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

                return parseExpressionStatement();
            }
            catch (Exception e)
            {
                // On error, skip to next statement
                //while (!atEnd() && !match(TokenTypes.EOF) && !peekMatch(0, TokenTypes.LParen))
                //{
                //    advance();
                //    Console.WriteLine(current());
                //}
                if (!atEnd()) advance();
                return null;
            }
        }
        private RangedStatement parseSet()
        {
            TextPointer stmtStart = previous().TextRange.Start;
    
            StmtSet stmt = new StmtSet(previous().Token.Loc);
            
            stmt.Identifier = consume(TokenTypes.TTWord, "expect identifier in 'set'");
            stmt.Value = parseExpression();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'set'");

            TextPointer stmtEnd = previous().TextRange.End;
            stmt.TextRange = new TextRange(stmtStart, stmtEnd);
            return stmt;
        }

        private RangedStatement parseNamespace()
        {
            TextPointer stmtStart = previous().TextRange.Start;
            StmtNamespace stmt = new StmtNamespace(previous().Token.Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'namespace'");
            TextPointer stmtEnd = previous().TextRange.End;
            stmt.TextRange = new TextRange(stmtStart, stmtEnd);
            return stmt;
        }

        private RangedStatement parseLoad()
        {
            TextPointer stmtStart = previous().TextRange.Start;
            StmtLoad stmt = new StmtLoad(previous().Token.Loc);
            stmt.Path = consume(TokenTypes.TTString, "expect filepath in 'load'");
            consume(TokenTypes.RParen, "expect enclosing ')' in 'load'");
            TextPointer stmtEnd = previous().TextRange.End;
            stmt.TextRange = new TextRange(stmtStart, stmtEnd);
            return stmt;
        }

        private RangedStatement parseEntry()
        {
            TextPointer stmtStart = previous().TextRange.Start;
            StmtEntry stmt = new StmtEntry(previous().Token.Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'entry'");
            TextPointer stmtEnd = previous().TextRange.End;
            stmt.TextRange = new TextRange(stmtStart, stmtEnd);
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
            TextPointer stmtStart = previous().TextRange.Start;

            StmtParameter fnNameRetType = parseParameter(0, false);
            StmtFnDeclaration stmtFnDeclaration = new StmtFnDeclaration(fnNameRetType.Loc);
            stmtFnDeclaration.Name = fnNameRetType.Alias.Token.Lexeme;
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
            while (match(TokenTypes.LParen))
            {
                var stmt = parseStatement();
                if (stmt != null) stmtFnDeclaration.Statements.Add(stmt);
            }
            consume(TokenTypes.RParen, "expect enclosing ')' in 'defn'");
            TextPointer stmtEnd = previous().TextRange.End;
            stmtFnDeclaration.TextRange = new TextRange(stmtStart, stmtEnd);
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
                return new DataType { TypeId = DataTypeEnum.Number };
            }
            if (match(TokenTypes.Boolean))
            {
                return new DataType { TypeId = DataTypeEnum.Boolean };
            }
            if (match(TokenTypes.Fn))
            {
                return new DataType { TypeId = DataTypeEnum.Fn };
            }
            if (match(TokenTypes.String))
            {
                return new DataType { TypeId = DataTypeEnum.String };
            }
            if (match(TokenTypes.Empty))
            {
                return new DataType { TypeId = DataTypeEnum.Empty };
            }
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = DataTypeEnum.Any };
            }
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = DataTypeEnum.Any };
            }
            if (match(TokenTypes.TypeInfo))
            {
                return new DataType { TypeId = DataTypeEnum.TypeInfo };
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = DataTypeEnum.NativeList };
                consume(TokenTypes.LCarat, "expect List<contained-typename> ");
                dt.ContainedType = parseDataType();
                consume(TokenTypes.RCarat, "expect enclosing '>'");
                return dt;
            }
            throw new Exception($"unrecognized type: {current()}");
        }
        private RangedStatement parseExpressionStatement()
        {
            TextPointer stmtStart = previous().TextRange.Start;

            StmtExpression stmtExpression = new StmtExpression(previous().Token.Loc);
            stmtExpression.Expression = parseCall();
            // Do not need to consume Rparen here since it is consumed in parseCall method
            TextPointer stmtEnd = previous().TextRange.End;
            stmtExpression.TextRange = new TextRange(stmtStart, stmtEnd);
            return stmtExpression;
        }

        private RangedExpression parseExpression()
        {
            if (match(TokenTypes.LParen))
            {
                RangedExpression expr;
                if (match(TokenTypes.Defn))
                {
                    expr = parseLiteralFunctionDeclaration();
                }
                else
                {
                    expr = parseCall();
                }
                return expr;
            }
            return parsePrimary();
        }

        private RangedExpression parseLiteralFunctionDeclaration()
        {
            TextPointer stmtStart = previous().TextRange.Start;

            ExprFnDeclaration exprFnDeclaration = new ExprFnDeclaration(previous().Token.Loc);
            if (match(current(), TokenTypes.TTWord))
            {
                StmtParameter fnNameRetType = parseParameter(0, false);
                exprFnDeclaration.Name = fnNameRetType.Alias.Token.Lexeme;
                exprFnDeclaration.ReturnType = fnNameRetType.DataType;
            }
            else
            {
                // It is lambda function
                exprFnDeclaration.Name = "[lambda]";
                exprFnDeclaration.ReturnType = new DataType { TypeId = DataTypeEnum.Empty };
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
            while (match(TokenTypes.LParen))
            {
                var stmt = parseStatement();
                if (stmt != null) exprFnDeclaration.Statements.Add(stmt);
            }
            consume(TokenTypes.RParen, "expect enclosing ')' in 'defn'");
            TextPointer stmtEnd = previous().TextRange.End;
            exprFnDeclaration.TextRange = new TextRange(stmtStart, stmtEnd);
            return exprFnDeclaration;
        }


        private RangedExpression parseCall()
        {
            TextPointer stmtStart = previous().TextRange.Start;

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
            TextPointer stmtEnd = previous().TextRange.End;
            exprCall.TextRange = new TextRange(stmtStart, stmtEnd);
            return exprCall;
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
            ExprLiteral exprLiteral = new ExprLiteral(previous().Token.Loc);
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
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo(new DataType { TypeId = DataTypeEnum.Empty });
                return exprLiteral;
            }
            if (match(TokenTypes.Any))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<object>();
                return exprLiteral;
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = DataTypeEnum.NativeList };
                consume(TokenTypes.LCarat, "expect List<contained-typename> ");
                dt.ContainedType = parseDataType();
                consume(TokenTypes.RCarat, "expect enclosing '>'");
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo(dt);
                return exprLiteral;
            }
            throw new Exception($"unexpected token in primary {current().Token}");
        }
    }
}
