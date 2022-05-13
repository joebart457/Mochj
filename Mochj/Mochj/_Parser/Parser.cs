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
    public class Parser : ParsingHelper
    {

        private NumberFormatInfo DefaultNumberFormat = new NumberFormatInfo { NegativeSign = "-" };
        public IEnumerable<Statement> Parse(IEnumerable<Token> tokens)
        {
            init(tokens);
            IList<Statement> statements = new List<Statement>();
            while (!atEnd() && !match(TokenTypes.EOF))
            {
                statements.Add(parseStatement());
            }
            return statements;
        }


        private Statement parseStatement()
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
                throw new Exception($"unexpected token where statement expected: {current()}");
            }
            
            Statement stmt = parseExpressionStatement();
            if (stmt is StmtExpression stmtExpr && stmtExpr.Expression is ExprFnDeclaration exprFn)
            {
                return new StmtFnDeclaration(exprFn.Loc)
                {
                    Label = exprFn.Label,
                    Name = exprFn.Name,
                    Parameters = exprFn.Parameters,
                    ReturnType = exprFn.ReturnType,
                    Statements = exprFn.Statements,
                };
            }
            return stmt;
        }
        private Statement parseSet()
        {
            StmtSet stmt = new StmtSet(previous().Loc);
            stmt.Identifier = consume(TokenTypes.TTWord, "expect identifier in 'set'").Lexeme;
            stmt.Value = parseExpression();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'set'");
            return stmt;
        }

        private Statement parseNamespace()
        {
            StmtNamespace stmt = new StmtNamespace(previous().Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'namespace'");
            return stmt;
        }

        private Statement parseLoad()
        {
            StmtLoad stmt = new StmtLoad(previous().Loc);
            stmt.Path = consume(TokenTypes.TTString, "expect filepath in 'load'").Lexeme;
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'load'");
            return stmt;
        }

        private Statement parseEntry()
        {
            StmtEntry stmt = new StmtEntry(previous().Loc);
            stmt.Symbol = parseSymbol();
            consume(TokenTypes.SemiColon, "expect enclosing ';' in 'entry'");
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
            while (!match(TokenTypes.SemiColon))
            {
                stmtFnDeclaration.Statements.Add(parseStatement());
            }
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
            if (match(TokenTypes.Any))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.Any };
            }
            if (match(TokenTypes.TypeInfo))
            {
                return new DataType { TypeId = Enums.DataTypeEnum.TypeInfo };
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
            StmtExpression stmtExpression = new StmtExpression(current().Loc);
            stmtExpression.Expression = parseExpression();
            return stmtExpression;
        }

        private Expression parseExpression()
        {
            return parseBinary();
        }

        private Expression parseBinary()
        {
            Expression expr = parseCall();
            while (match(TokenTypes.DoubleQuestionMark) || match(TokenTypes.DoubleDot) || match(TokenTypes.Colon))
            {
                if (match(previous(), TokenTypes.DoubleQuestionMark))
                {
                    expr = new ExprNullableSwitch(previous().Loc, expr, parseBinary());
                } else if (match(previous(), TokenTypes.DoubleDot))
                {
                    var exprGetArg = new ExprGetArgument(previous().Loc, expr, false);
                    exprGetArg.Identifier = consume(TokenTypes.TTWord).Lexeme;
                    expr = exprGetArg;
                } else
                {
                    var exprGetArg = new ExprGetArgument(previous().Loc, expr, true);
                    exprGetArg.Identifier = consume(TokenTypes.TTWord).Lexeme;
                    expr = exprGetArg;
                }
            }
            return expr;
        }

        private Expression parseLiteralFunctionDeclaration()
        {
            ExprFnDeclaration exprFnDeclaration = new ExprFnDeclaration(previous().Loc);
            if (match(current(), TokenTypes.TTWord))
            {
                StmtParameter fnNameRetType = parseParameter(0, false);
                exprFnDeclaration.Name = fnNameRetType.Alias;
                exprFnDeclaration.ReturnType = fnNameRetType.DataType;
            } else
            {
                // It is lambda function
                exprFnDeclaration.Name = "[lambda]";
                exprFnDeclaration.ReturnType = new DataType { TypeId = Enums.DataTypeEnum.Empty };
            }
            
            exprFnDeclaration.Parameters = new List<StmtParameter>();
            int paramCount = 0;
            while (match(TokenTypes.LBracket))
            {
                exprFnDeclaration.Parameters.Add(parseParameter(paramCount, true));
                consume(TokenTypes.RBracket, "expect enclosing ']' for parameter");
                paramCount++;
            }
            exprFnDeclaration.Statements = new List<Statement>();
            while (!match(TokenTypes.RParen))
            {
                exprFnDeclaration.Statements.Add(parseStatement());
            }
            return exprFnDeclaration;
        }
       
        
        private Expression parseCall()
        {
            if (match(TokenTypes.LParen))
            {
                if (match(TokenTypes.Defn))
                {
                    return parseLiteralFunctionDeclaration();
                }
                Symbol sym = parseSymbol();
                ExprCall exprCall = new ExprCall(previous().Loc);
                exprCall.Symbol = sym;
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
            return parsePrimary();
        }

        private ExprArgument parseArgument(int argCount, ref bool wasExplicitlyNamed)
        {
            ExprArgument exprArgument = new ExprArgument(previous().Loc);
            if (wasExplicitlyNamed)
            {
                consume(TokenTypes.DoubleDash, "expect explicit parameter name '--<param-name>'");
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            } else if (match(TokenTypes.DoubleDash))
            {
                exprArgument.ParameterAlias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            } else
            {
                exprArgument.Position = argCount;
            }
            exprArgument.Value = parseExpression();
            return exprArgument;
        }

        private Expression parsePrimary()
        {
            ExprLiteral exprLiteral = new ExprLiteral(current().Loc);
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
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo(new DataType { TypeId = Enums.DataTypeEnum.Empty });
                return exprLiteral;
            }
            if (match(TokenTypes.Any))
            {
                exprLiteral.Value = QualifiedObjectBuilder.BuildTypeInfo<object>();
                return exprLiteral;
            }
            if (match(TokenTypes.NativeList))
            {
                DataType dt = new DataType { TypeId = Enums.DataTypeEnum.NativeList };
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
