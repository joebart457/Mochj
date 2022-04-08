using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj.Builders;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Interpreter.Models;
using Mochj.IDE._Parser;
using Mochj.IDE._Parser.Models.Statements;
using Mochj.IDE._Tokenizer;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Environment = Mochj._Storage.Environment;

namespace Mochj.IDE.Bright
{
    class BrightSession
    {
        public class ScriptExcecutionInfo
        {
            public ExecutionInfo ExecutionInfo { get; set; }
            public List<RangedToken> Tokens { get; set; }
        }

        private Dictionary<string, TokenClassifierEnum> Classifiers = new Dictionary<string, TokenClassifierEnum>();

        public ScriptExcecutionInfo LastRunInfo { get; private set; }

        public BrightSession()
        {
            Classifiers[TokenTypes.EOLComment] = TokenClassifierEnum.Comment;

            Classifiers[TokenTypes.TTString] = TokenClassifierEnum.String;
            Classifiers[TokenTypes.TTUnsignedInteger] = TokenClassifierEnum.Number;
            Classifiers[TokenTypes.TTDouble] = TokenClassifierEnum.Number;
            Classifiers[TokenTypes.TTFloat] = TokenClassifierEnum.Number;
            Classifiers[TokenTypes.TTInteger] = TokenClassifierEnum.Number;

            Classifiers[TokenTypes.LiteralTrue] = TokenClassifierEnum.Keyword_1;
            Classifiers[TokenTypes.LiteralFalse] = TokenClassifierEnum.Keyword_1;

            Classifiers[TokenTypes.Set] = TokenClassifierEnum.Keyword_2;
            Classifiers[TokenTypes.Defn] = TokenClassifierEnum.Keyword_2;
            Classifiers[TokenTypes.Load] = TokenClassifierEnum.Keyword_2;

            Classifiers[TokenTypes.NativeList] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Number] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.String] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Boolean] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Fn] = TokenClassifierEnum.Keyword_3;

            Classifiers[TokenTypes.Any] = TokenClassifierEnum.Keyword_4;

        }

        public async Task Run(TextPointer pointer)
        {
            var tokens = RangedTokenFactory.TokenizeFromPosition(pointer);
            var stmts = new InformativeParser().Parse(tokens.Where(t => !t.Token.Type.StartsWith("WhiteSpace") && t.Token.Type != TokenTypes.EOLComment), out _);
            var interpreter = new InformedInterpreter(DefaultEnvironmentBuilder.Build(true));
            var info = await Task.Run( () => interpreter.Interpret(stmts.ToList()));
            LastRunInfo = new ScriptExcecutionInfo 
            { 
                ExecutionInfo = info, 
                Tokens = ClassifyTokens(tokens),
            };
        }

        public Environment GetContextForPointer(TextPointer pointer)
        {
            if (LastRunInfo == null ||
                LastRunInfo.ExecutionInfo == null ||
                LastRunInfo.ExecutionInfo.Environment == null) 
                return null;
            var x = LastRunInfo.ExecutionInfo.Spans.Select(span => span.GetInnerMostContainedSpan(pointer)).ToList();
            var containedSpan = LastRunInfo.ExecutionInfo.Spans.Select(span => span.GetInnerMostContainedSpan(pointer)).Where(s => s!= null).FirstOrDefault();
            if (containedSpan == null) return null;
            return containedSpan.Environment;
        }

        public List<string> GetAutoCompleteForSymbol(Symbol symbol, TextPointer pointer)
        {
            var x = GetContextForPointer(pointer);
            Environment env = GetContextForPointer(pointer) ?? DefaultEnvironmentBuilder.Build(true);
            
            var envTarget = SymbolResolverHelper.ResolveToNamespaceOrNull(env, symbol);
            if (envTarget == null)
            {
                // Try one more time without the end symbol as this may 
                // be an incomplete word
                var trimmedSymbol = new Symbol();
                string hint = null;
                if (symbol != null && symbol.Names != null && symbol.Names.Any())
                {
                    hint = symbol.Names.Last();
                    trimmedSymbol.Names = symbol.Names.Where(n => n != hint).ToList();
                }
                envTarget = SymbolResolverHelper.ResolveToNamespaceOrNull(env, trimmedSymbol);
                if (envTarget == null) 
                    return new List<string>();
                return GetLookupList(envTarget).Where(s => hint == null ? true : s.StartsWith(hint)).ToList();
            }
            return GetLookupList(envTarget);
        }

        public List<RangedToken> ClassifyTokens(List<RangedToken> tokens)
        {
            foreach(var token in tokens)
            {
                if (Classifiers.TryGetValue(token.Token.Type, out var classifier))
                {
                    token.Classifier = classifier;
                }
            }
            return tokens;
        }

        public string GetTooltipForPointer(TextPointer pointer)
        {
            if (LastRunInfo == null || LastRunInfo.Tokens == null || !LastRunInfo.Tokens.Any()) return null;
            return LastRunInfo.Tokens.Where(token => token.TextRange.Contains(pointer)).FirstOrDefault()?.Message;
        }


        private static List<string> GetLookupList(Environment env)
        {
            if (env == null) return new List<string>();
            List<string> lookup = new List<string>();
            do
            {
                env.Lookup.ForEach(kv => lookup.Add(kv.Key));
                env = env.Enclosing;
            } while (env != null);
            return lookup;
        }


    }
}
