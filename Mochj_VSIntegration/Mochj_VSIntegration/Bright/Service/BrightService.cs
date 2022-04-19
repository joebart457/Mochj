using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj.IDE._Tokenizer;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Bright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mochj_VSIntegration.Bright.Service
{
    internal static class BrightService
    {
        private static BrightSession _session = new BrightSession();

        public static BrightSession.ScriptExecutionInfo GetLastRunInfo()
        {
            return _session.LastRunInfo;
        }

        public static Task ReRunSessionAsync(ITextSnapshot snapshot)
        {
            return Task.Run(() => _session.Run(snapshot));
        }
        public static List<CompletionSet> AugmentCompletionSession(ICompletionSession session, SnapshotPoint triggerPoint)
        {

            var tokens = RangedTokenFactory.TokenizeFromSnapshot(session.TextView.TextSnapshot);
            var index = tokens.FindIndex(t => t.TextRange.Contains(triggerPoint));
            var containedToken = tokens.Find(t => t.TextRange.Contains(triggerPoint));

            if (index < 0 || containedToken == null) return new List<CompletionSet>();

            var completions = _session.GetAutoCompleteForSymbol(GetPrecedingSymbol(tokens, index, out var offset), triggerPoint).Select(x => new Completion(x));
            var completionSets = new List<CompletionSet>();
            completionSets.Add(
                new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindAtPosition(session, containedToken, offset),
                completions,
                null)); ;
            return completionSets;

        }

        private static Symbol GetPrecedingSymbol(List<RangedToken> tokens, int index, out int offset)
        {
            offset = 0;
            if (tokens[index].Token.Type == TokenTypes.Dot)
            {
                index--;
                offset = 1;
            }
            List<string> symbols = new List<string>();

            if (index < 0) return null;
            var currentToken = tokens[index];
            RangedToken separator;
            do
            {
                symbols.Add(currentToken.Token.Lexeme);
                index--;
                if (index < 0)
                {
                    break;
                }
                separator = tokens.ElementAt(index);
                index--;
                if (index < 0)
                {
                    break;
                }
                currentToken = tokens.ElementAt(index);

            } while (separator.Token.Type == TokenTypes.Dot && currentToken.Token.Type == TokenTypes.TTWord);

            symbols.Reverse();
            return new Symbol { Names = symbols };
        }

        private static ITrackingSpan FindAtPosition(ICompletionSession session, RangedToken token, int snapshotOffset)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - snapshotOffset;
            return currentPoint.Snapshot.CreateTrackingSpan(new Span((int)token.Token.Loc.X - token.Token.Lexeme.Length, token.Token.Lexeme.Length), SpanTrackingMode.EdgeInclusive);
        }
    }
}
