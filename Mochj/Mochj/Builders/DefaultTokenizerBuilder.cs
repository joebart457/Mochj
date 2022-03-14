using Mochj._Tokenizer;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Builders
{
    public static class DefaultTokenizerBuilder
    {
        public static Tokenizer Build()
        {

            TokenizerSettings settings = new TokenizerSettings
            {
                StringCatalystExcluded = ") ",
                StringCatalystEscapable = ") \\"
            };

            List<TokenizerRule> rules = new List<TokenizerRule>();
            rules.Add(new TokenizerRule(TokenTypes.StringEnclosing, "\""));
            rules.Add(new TokenizerRule(TokenTypes.StringEnclosing, "'"));
            rules.Add(new TokenizerRule(TokenTypes.StringCatalyst, "$"));
            rules.Add(new TokenizerRule(TokenTypes.EOLComment, "---"));
            rules.Add(new TokenizerRule(TokenTypes.EOLComment, "//"));
            rules.Add(new TokenizerRule(TokenTypes.MLCommentStart, "/*"));
            rules.Add(new TokenizerRule(TokenTypes.MLCommentEnd, "*/"));
            rules.Add(new TokenizerRule(TokenTypes.EOF, "EOF"));

            rules.Add(new TokenizerRule(TokenTypes.Number, "number"));
            rules.Add(new TokenizerRule(TokenTypes.String, "string"));
            rules.Add(new TokenizerRule(TokenTypes.Boolean, "boolean"));
            rules.Add(new TokenizerRule(TokenTypes.Fn, "function"));
            rules.Add(new TokenizerRule(TokenTypes.NativeList, "List"));
            rules.Add(new TokenizerRule(TokenTypes.Empty, "empty"));
            rules.Add(new TokenizerRule(TokenTypes.Any, "any"));

            rules.Add(new TokenizerRule(TokenTypes.Set, "set"));
            rules.Add(new TokenizerRule(TokenTypes.Defn, "defn"));
            rules.Add(new TokenizerRule(TokenTypes.Namespace, "namespace"));
            rules.Add(new TokenizerRule(TokenTypes.Load, "load"));
            rules.Add(new TokenizerRule(TokenTypes.Entry, "entry"));
            rules.Add(new TokenizerRule(TokenTypes.Use, "use"));

            rules.Add(new TokenizerRule(TokenTypes.LiteralTrue, "true"));
            rules.Add(new TokenizerRule(TokenTypes.LiteralFalse, "false"));

            rules.Add(new TokenizerRule(TokenTypes.Colon, ":"));
            rules.Add(new TokenizerRule(TokenTypes.SemiColon, ";"));
            rules.Add(new TokenizerRule(TokenTypes.Comma, ","));
            rules.Add(new TokenizerRule(TokenTypes.Not, "!"));
            rules.Add(new TokenizerRule(TokenTypes.Dot, "."));
            rules.Add(new TokenizerRule(TokenTypes.Ampersand, "&"));
            rules.Add(new TokenizerRule(TokenTypes.Pipe, "|"));
            rules.Add(new TokenizerRule(TokenTypes.Plus, "+"));
            rules.Add(new TokenizerRule(TokenTypes.Dash, "-"));
            rules.Add(new TokenizerRule(TokenTypes.Asterisk, "*"));
            rules.Add(new TokenizerRule(TokenTypes.ForwardSlash, "/"));
            rules.Add(new TokenizerRule(TokenTypes.Equal, "="));
            rules.Add(new TokenizerRule(TokenTypes.LParen, "("));
            rules.Add(new TokenizerRule(TokenTypes.RParen, ")"));
            rules.Add(new TokenizerRule(TokenTypes.LBracket, "["));
            rules.Add(new TokenizerRule(TokenTypes.RBracket, "]"));
            rules.Add(new TokenizerRule(TokenTypes.LCurly, "{"));
            rules.Add(new TokenizerRule(TokenTypes.RCurly, "}"));
            rules.Add(new TokenizerRule(TokenTypes.LCarat, "<"));
            rules.Add(new TokenizerRule(TokenTypes.RCarat, ">"));
            rules.Add(new TokenizerRule(TokenTypes.UpCarat, "^"));

            rules.Add(new TokenizerRule(TokenTypes.And, "&&"));
            rules.Add(new TokenizerRule(TokenTypes.Or, "||"));
            rules.Add(new TokenizerRule(TokenTypes.EqualEqual, "=="));
            rules.Add(new TokenizerRule(TokenTypes.NotEqual, "!="));
            rules.Add(new TokenizerRule(TokenTypes.LessEqual, "<="));
            rules.Add(new TokenizerRule(TokenTypes.GreaterEqual, ">="));
            rules.Add(new TokenizerRule(TokenTypes.DoubleLCarat, "<<"));
            rules.Add(new TokenizerRule(TokenTypes.DoubleRCarat, ">>"));
            rules.Add(new TokenizerRule(TokenTypes.DoubleDash, "--"));

            return new Tokenizer(rules, settings);
        }
    }
}
