using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj.Builders;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Interpreter.Models;
using Mochj.IDE._Parser;
using Mochj.IDE._Tokenizer;
using Mochj.IDE._Tokenizer.Models;
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

        public ScriptExcecutionInfo LastRunInfo { get; private set; }
        public void Run(TextPointer pointer)
        {
            var tokens = RangedTokenFactory.TagFromPosition(pointer);
            var stmts = new InformativeParser().Parse(tokens, out _);
            var interpreter = new InformedInterpreter(DefaultEnvironmentBuilder.Build(true));
            var info = interpreter.Interpret(stmts.ToList());
            LastRunInfo = new ScriptExcecutionInfo { ExecutionInfo = info, Tokens = tokens };
        }

        public Environment GetContextForPointer(TextPointer pointer)
        {
            if (LastRunInfo == null ||
                LastRunInfo.ExecutionInfo == null ||
                LastRunInfo.ExecutionInfo.Environment == null) 
                return null;

            var containedSpan = LastRunInfo.ExecutionInfo.Spans.Where(span => span.GetInnerMostContainedSpan(pointer) != null).FirstOrDefault();
            if (containedSpan == null) return null;
            return containedSpan.Environment;
        }

        public List<string> GetAutoCompleteForSymbol(Symbol symbol, TextPointer pointer)
        {
            Environment env = LastRunInfo?.ExecutionInfo?.Environment;
            if (env != null)
            {
                if (symbol == null || symbol.Names == null || !symbol.Names.Any())
                {
                    env = GetContextForPointer(pointer);
                }
            } else
            {
                env = DefaultEnvironmentBuilder.Build(true);
            }
            
            var envTarget = SymbolResolverHelper.ResolveToNamespaceOrNull(env, symbol);
            if (envTarget == null) return new List<string>();
            return envTarget.Lookup.Select(kv => kv.Key).ToList();
        }

        public List<string> GetAutoCompleteForSymbol(Symbol symbol, TextPointer pointer, string hint)
        {
            if (hint == null) hint = string.Empty;
            var completionSession = GetAutoCompleteForSymbol(symbol, pointer);
            return completionSession.Where(h => h.ToLower().StartsWith(hint.ToLower())).ToList();
        }

    }
}
