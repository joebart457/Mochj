using Microsoft.VisualStudio.Text;
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
using Environment = Mochj._Storage.Environment;

namespace Mochj.IDE.Bright
{
    class BrightSession
    {
        public class ScriptExecutionInfo
        {
            public ExecutionInfo ExecutionInfo { get; set; }
            public List<RangedToken> Tokens { get; set; }
        }

        private Dictionary<string, TokenClassifierEnum> Classifiers = new Dictionary<string, TokenClassifierEnum>();

        public ScriptExecutionInfo LastRunInfo { get; private set; }

        public BrightSession()
        {
            Classifiers[TokenTypes.EOLComment] = TokenClassifierEnum.Comment_1;

            Classifiers[TokenTypes.TTString] = TokenClassifierEnum.String_1;
            Classifiers[TokenTypes.TTUnsignedInteger] = TokenClassifierEnum.Number_1;
            Classifiers[TokenTypes.TTDouble] = TokenClassifierEnum.Number_1;
            Classifiers[TokenTypes.TTFloat] = TokenClassifierEnum.Number_1;
            Classifiers[TokenTypes.TTInteger] = TokenClassifierEnum.Number_1;

            Classifiers[TokenTypes.LiteralTrue] = TokenClassifierEnum.Boolean_1;
            Classifiers[TokenTypes.LiteralFalse] = TokenClassifierEnum.Boolean_1;

            Classifiers[TokenTypes.Set] = TokenClassifierEnum.Keyword_2;
            Classifiers[TokenTypes.Defn] = TokenClassifierEnum.Keyword_2;
            Classifiers[TokenTypes.Load] = TokenClassifierEnum.Keyword_2;

            Classifiers[TokenTypes.NativeList] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Number] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.String] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Boolean] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Fn] = TokenClassifierEnum.Keyword_3;
            Classifiers[TokenTypes.Empty] = TokenClassifierEnum.Keyword_3;

            Classifiers[TokenTypes.Any] = TokenClassifierEnum.Keyword_4;

        }

        public void Run(ITextSnapshot snapshot)
        {
            var tokens = RangedTokenFactory.TokenizeFromSnapshot(snapshot);
            var stmts = new InformativeParser2().Parse(tokens.Where(t => !t.Token.Type.StartsWith("WhiteSpace") && t.Token.Type != TokenTypes.EOLComment));
            var interpreter = new InformedInterpreter(DefaultEnvironmentBuilder.Build(true));
            var info = interpreter.Interpret(stmts.ToList());
            LastRunInfo = new ScriptExecutionInfo 
            { 
                ExecutionInfo = info, 
                Tokens = ClassifyTokens(tokens),
            };
        }

        public Environment GetContextForPointer(SnapshotPoint point)
        {
            if (LastRunInfo == null ||
                LastRunInfo.ExecutionInfo == null ||
                LastRunInfo.ExecutionInfo.Environment == null) 
                return null;
            var x = LastRunInfo.ExecutionInfo.Spans.Select(span => span.GetInnerMostContainedSpan(point)).ToList();
            var containedSpan = LastRunInfo.ExecutionInfo.Spans.Select(span => span.GetInnerMostContainedSpan(point)).Where(s => s!= null).FirstOrDefault();
            if (containedSpan == null) return null;
            return containedSpan.Environment;
        }

        public List<string> GetAutoCompleteForSymbol(Symbol symbol, SnapshotPoint point)
        {
            Environment env = GetContextForPointer(point) ?? DefaultEnvironmentBuilder.Build(true);
            
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
            foreach(var token in tokens.Where(t => t.Classifier == TokenClassifierEnum.Unkown))
            {
                if (Classifiers.TryGetValue(token.Token.Type, out var classifier))
                {
                    token.Classifier = classifier;
                }
            }
            return tokens;
        }

        public string GetTooltipForPointer(SnapshotPoint point)
        {
            if (LastRunInfo == null || LastRunInfo.Tokens == null || !LastRunInfo.Tokens.Any()) return null;
            return LastRunInfo.Tokens.Where(token => token.TextRange.Contains(point)).FirstOrDefault()?.Message;
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
