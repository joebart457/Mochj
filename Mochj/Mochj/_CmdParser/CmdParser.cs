using Mochj._Parser.Helpers;
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

namespace Mochj._CmdParser
{
    class CmdParser: ParsingHelper
    {
        private NumberFormatInfo DefaultNumberFormat = new NumberFormatInfo { NegativeSign = "-" };

        public Args ParseCommandLineArgs(string[] args)
        {
            _Tokenizer.Tokenizer tokenizer = DefaultTokenizerBuilder.Build();
            IEnumerable<Token> tokens = tokenizer.Tokenize(string.Join(" ", args));
            init(tokens);

            Args arguments = new Args();
            int argc = 0;
            bool argWasExplicitlyNamed = false;
            while (!atEnd())
            {
                Argument argument = parseArgument(argc, ref argWasExplicitlyNamed);
                if (string.IsNullOrEmpty(argument.Alias))
                {
                    arguments.AddVariadicArgument(argument);
                } else
                {
                    arguments.Add(argument);
                }
                argc++;
            }

            return arguments;
        }

        private Argument parseArgument(int argCount, ref bool wasExplicitlyNamed)
        {
            Argument argument = new Argument();
            if (wasExplicitlyNamed)
            {
                consume(TokenTypes.DoubleDash, "expect explicit parameter name '--<param-name>'");
                argument.Alias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            }
            else if (match(TokenTypes.DoubleDash))
            {
                argument.Alias = consume(TokenTypes.TTWord, "expect parameter name after '--'").Lexeme;
                wasExplicitlyNamed = true;
            }

            // Little bit hacky here
            // notice how we always provide position, even if argument is aliased
            // this allows it to be added to position dictionary and thus be passed to ResolveArguments
            // this means specifying an already positional specified argument will result in it being
            // doubly defined
            argument.Position = argCount;

            argument.Value = parseLiteral();
            return argument;
        }

        private QualifiedObject parseLiteral()
        {
            if (match(TokenTypes.LiteralFalse))
            {
                return QualifiedObjectBuilder.BuildBoolean(false);
            }
            if (match(TokenTypes.LiteralTrue))
            {
                return QualifiedObjectBuilder.BuildBoolean(true);
            }
            if (match(TokenTypes.TTInteger))
            {
                return QualifiedObjectBuilder.BuildNumber(int.Parse(previous().Lexeme, DefaultNumberFormat));
            }
            if (match(TokenTypes.TTUnsignedInteger))
            {
                return QualifiedObjectBuilder.BuildNumber(uint.Parse(previous().Lexeme, DefaultNumberFormat));
            }
            if (match(TokenTypes.TTFloat))
            {
                return QualifiedObjectBuilder.BuildNumber(float.Parse(previous().Lexeme, DefaultNumberFormat));
            }
            if (match(TokenTypes.TTDouble))
            {
                return QualifiedObjectBuilder.BuildNumber(double.Parse(previous().Lexeme, DefaultNumberFormat));
            }
            if (match(TokenTypes.TTString))
            {
                return QualifiedObjectBuilder.BuildString(previous().Lexeme);
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
                    return QualifiedObjectBuilder.BuildNumber(int.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                }
                if (match(TokenTypes.TTUnsignedInteger))
                {
                    return QualifiedObjectBuilder.BuildNumber(uint.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                }
                if (match(TokenTypes.TTFloat))
                {
                    return QualifiedObjectBuilder.BuildNumber(float.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                }
                if (match(TokenTypes.TTDouble))
                {
                    return QualifiedObjectBuilder.BuildNumber(double.Parse("-" + previous().Lexeme, DefaultNumberFormat));
                }
                throw new Exception($"unexpected token while parsing negative {current()}");
            }
            
            throw new Exception($"unexpected token while parsing literal {current()}");
        }
    }
}
