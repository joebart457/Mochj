using Mochj._Parser.Helpers;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser
{
    public class ItemizedParser : ParsingHelper
    {

        private NumberFormatInfo DefaultNumberFormat = new NumberFormatInfo { NegativeSign = "-" };

        public Expression ParseExpression(string text)
        {

            var tokens = DefaultTokenizerBuilder.Build().Tokenize(text).Where(x => x.Type != TokenTypes.EOF);
            init(tokens);

            Expression expr = parseExpression();
            if (!atEnd())
            {
                throw new Exception($"parsed expression but tokens still remain: {current()} ...");
            }
            return expr;
        }

        public UserDefinedFunction ParseFunction(string text)
        {

            var tokens = DefaultTokenizerBuilder.Build().Tokenize(text).Where(x => x.Type != TokenTypes.EOF);
            init(tokens);

            UserDefinedFunction fn = null;
            if (match(TokenTypes.LParen))
            {
                if (match(TokenTypes.Defn))
                {
                    _Interpreter.Interpreter i = new _Interpreter.Interpreter(DefaultEnvironmentBuilder.Default);
                    fn = i.Accept(parseLiteralFunctionDeclaration()).Object as UserDefinedFunction;
                    if (!atEnd())
                    {
                        throw new Exception($"parsed function but tokens still remain: {current()} ...");
                    }
                    return fn;
                } else
                {
                    throw new Exception($"expected function declaration but got token {current()}");
                }
            }
            throw new Exception($"expected function declaration but got token {current()}");
        }


        private Statement parseStatement()
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
        private Statement parseSet()
        {
            StmtSet stmt = new StmtSet(previous().Loc);
            stmt.Identifier = consume(TokenTypes.TTWord, "expect identifier in 'set'").Lexeme;
            stmt.Value = parseExpression();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'set'");
            return stmt;
        }

        private Statement parseNamespace()
        {
            StmtNamespace stmt = new StmtNamespace(previous().Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'namespace'");
            return stmt;
        }

        private Statement parseLoad()
        {
            StmtLoad stmt = new StmtLoad(previous().Loc);
            stmt.Path = consume(TokenTypes.TTString, "expect filepath in 'load'").Lexeme;
            consume(TokenTypes.RParen, "expect enclosing ')' in 'load'");
            return stmt;
        }

        private Statement parseEntry()
        {
            StmtEntry stmt = new StmtEntry(previous().Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.RParen, "expect enclosing ')' in 'entry'");
            return stmt;
        }

        private Symbol parseSymbol()
        {
            IList<string> names = new List<string>();
            Location location;
            do
            {
                Token token = consume(TokenTypes.TTWord, "expect name symbol");
                location = token.Loc;
                names.Add(token.Lexeme);
            } while (match(TokenTypes.Dot));
            return new Symbol { Location = location, Names = names };
        }

        private Statement parseFunctionDeclaration()
        {
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
            stmtFnDeclaration.Statements = new List<Statement>();
            while (match(TokenTypes.LParen))
            {
                stmtFnDeclaration.Statements.Add(parseStatement());
            }
            consume(TokenTypes.RParen, "expect enclosing ')' in 'defn'");
            return stmtFnDeclaration;
        }

        private StmtParameter parseParameter(int paramCount, bool allowDefaultValue = true)
        {
            StmtParameter stmtParameter = new StmtParameter();
            Token alias = consume(TokenTypes.TTWord, "expect parameter name");
            stmtParameter.Loc = alias.Loc;
            stmtParameter.Alias = alias.Lexeme;
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
                return new DataType { TypeId = Enums.DataTypeEnum.Number };
            }
            if (match(TokenTypes.Boolean))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Boolean };
            }
            if (match(TokenTypes.Fn))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Fn };
            }
            if (match(TokenTypes.String))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.String };
            }
            if (match(TokenTypes.Empty))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Empty };
            }
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Any };
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = Enums.DataTypeEnum.NativeList };
                consume(TokenTypes.LCarat, "expect List<contained-typename> ");
                dt.ContainedType = parseDataType();
                consume(TokenTypes.RCarat, "expect enclosing '>'");
                return dt;
            }
            throw new Exception($"unrecognized type: {current()}");
        }
        private Statement parseExpressionStatement()
        {
            StmtExpression stmtExpression = new StmtExpression(previous().Loc);
            stmtExpression.Expression = parseCall();
            // Do not need to consume Rparen here since it is consumed in parseCall method
            return stmtExpression;
        }

        private Expression parseExpression()
        {
            if (match(TokenTypes.LParen))
            {
                Expression expr;
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

        private ExprFnDeclaration parseLiteralFunctionDeclaration()
        {
            StmtParameter fnNameRetType = parseParameter(0, false);
            ExprFnDeclaration exprFnDeclaration = new ExprFnDeclaration(fnNameRetType.Loc);
            exprFnDeclaration.Name = fnNameRetType.Alias;
            exprFnDeclaration.ReturnType = fnNameRetType.DataType;
            exprFnDeclaration.Parameters = new List<StmtParameter>();
            int paramCount = 0;
            while (match(TokenTypes.LBracket))
            {
                exprFnDeclaration.Parameters.Add(parseParameter(paramCount, true));
                consume(TokenTypes.RBracket, "expect enclosing ']' for parameter");
                paramCount++;
            }
            exprFnDeclaration.Statements = new List<Statement>();
            while (match(TokenTypes.LParen))
            {
                exprFnDeclaration.Statements.Add(parseStatement());
            }
            consume(TokenTypes.RParen, "expect enclosing ')' in 'defn'");
            return exprFnDeclaration;
        }


        private Expression parseCall()
        {
            ExprCall exprCall = new ExprCall(previous().Loc);
            exprCall.Symbol = parseSymbol();
            exprCall.Arguments = new List<ExprArgument>();
            bool requireExplicitTypes = false;
            int argCount = 0;
            while (!match(TokenTypes.RParen))
            {
                exprCall.Arguments.Add(parseArgument(argCount, ref requireExplicitTypes));
                argCount++;
            }
            return exprCall;
        }

        private ExprArgument parseArgument(int argCount, ref bool wasExplicitlyNamed)
        {
            ExprArgument exprArgument = new ExprArgument(previous().Loc);
            if (wasExplicitlyNamed)
            {
                consume(TokenTypes.DoubleDash, "expect explicit parameter name '--<param-name>'");
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            }
            else if (match(TokenTypes.DoubleDash))
            {
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            }
            else
            {
                exprArgument.Position = argCount;
            }
            exprArgument.Value = parseExpression();
            return exprArgument;
        }
        private Expression parsePrimary()
        {
            ExprLiteral exprLiteral = new ExprLiteral(previous().Loc);
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
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(int.Parse(previous().Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTUnsignedInteger))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(uint.Parse(previous().Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTFloat))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(float.Parse(previous().Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTDouble))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(double.Parse(previous().Lexeme, DefaultNumberFormat));
                return exprLiteral;
            }
            if (match(TokenTypes.TTString))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildString(previous().Lexeme);
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
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(int.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTUnsignedInteger))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(uint.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTFloat))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(float.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                if (match(TokenTypes.TTDouble))
                {
                    exprLiteral.Value = QualifiedObjectBuilder.BuildNumber(double.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                    return exprLiteral;
                }
                throw new Exception($"unexpected token while parsing negative {current()}");
            }
            if (match(current(), TokenTypes.TTWord))
            {
                ExprIdentifier exprIdentifier = new ExprIdentifier(previous().Loc);
                exprIdentifier.Symbol = parseSymbol();
                return exprIdentifier;
            }
            throw new Exception($"unexpected token in primary {current()}");
        }
    }
}
