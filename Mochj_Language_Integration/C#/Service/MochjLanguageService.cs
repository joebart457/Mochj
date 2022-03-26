using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Mochj._Interpreter.Helpers;
using Mochj._Parser.Models;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.Models;
using MochjLanguage.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MochjLanguage.Service
{
    static class MochjLanguageService
    {

        public static List<CompletionSet> AugmentCompletionSession(ICompletionSession session, SnapshotPoint triggerPoint)
        {
            var completionSets = new List<CompletionSet>();
            if (triggerPoint == null) return completionSets;

            var completions = new List<Completion>();

            var tokenizer = MochjScriptTokenizerBuilder.Build();
            var tokens = tokenizer.Tokenize(session.TextView.TextSnapshot.GetText());
            int position = triggerPoint.Position;
            var tokensList = tokens.ToList();
            
            var symbol = GetFullSymbol(ref tokensList, ref position, out var offset);
            if (symbol == null) return completionSets;

            if (MochjScriptingService.RunScriptInBackground(session.TextView.TextSnapshot.GetText()))
            {
                completions = GetCompletionListForSymbol(symbol).Select(str => new Completion(str, str, str, null, null)).ToList();
            }

            var contained = GetContainedToken(tokensList, position);         
            completionSets.Add(
                new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindAtPosition(session, contained, 1 - offset),
                completions,
                null));
            return completionSets;
        }

        private static ITrackingSpan FindAtPosition(ICompletionSession session, Token token, int snapshotOffset)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - snapshotOffset;
            return currentPoint.Snapshot.CreateTrackingSpan(new Span((int)token.Loc.X - token.Lexeme.Length, token.Lexeme.Length), SpanTrackingMode.EdgeInclusive);
        }

        private static List<string> GetCompletionListForSymbol(Symbol symbol)
        {
            return MochjScriptingService.GetEnvMembersForSymbol(DropSymbolBase(symbol));
        }

        private static Symbol DropSymbolBase(Symbol symbol)
        {
            if (symbol == null || symbol.Names == null || !symbol.Names.Any()) return symbol;
            symbol.Names.RemoveAt(symbol.Names.Count - 1);
            return symbol;
        }

        private static Symbol GetFullSymbol(ref List<Token> tokens, ref int position, out int offset)
        {
            offset = 0;
            Token containedToken = GetContainedToken(tokens, position);
            if (containedToken == null) return null;
            if (containedToken.Type == TokenTypes.Dot)
            {
                int i = tokens.IndexOf(containedToken);
                if (i == -1) return null;
                position++;
                Token manufacturedWord = new Token(TokenTypes.TTWord, "x", containedToken.Loc.Y, (uint)position);
                tokens.Insert(i + 1, manufacturedWord);
                containedToken = manufacturedWord;
                offset = 1;
            }
            if (containedToken.Type != TokenTypes.TTWord) return null;
            int index = tokens.IndexOf(containedToken);

            if (index == -1) return null;
            if (index == 0)
            {
                return new Symbol { Names = new List<string> { containedToken.Lexeme } };
            }
            else
            {
                List<string> symbols = new List<string>();

                var currentToken = containedToken;
                Token separator;
                do
                {
                    symbols.Add(currentToken.Lexeme);
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

                } while (separator.Type == TokenTypes.Dot && currentToken.Type == TokenTypes.TTWord);
                symbols.Reverse();
                return new Symbol { Names = symbols };
            }
        }

        private static Token GetContainedToken(List<Token> tokens, int position)
        {
            if (!tokens.Any()) return null;
            Token previousToken = tokens.First();
            uint previousPosition = previousToken.Loc.X;
            for (int i = 0; i < tokens.Count; i++)
            {
                var currentToken = tokens.ElementAt(i);
                if (position == currentToken.Loc.X) return currentToken;
                if (position > previousPosition && position < currentToken.Loc.X)
                {
                    return previousToken;
                }
                previousPosition = currentToken.Loc.X;
            }
            return null;
        }

        public static Symbol GetFullSymbolFromTokenStream(Token token, List<Token> tokens)
        {
            if (token.Type != TokenTypes.TTWord) return null;
            int index = tokens.IndexOf(token);
            if (index == -1) return null;
            if (index == 0)
            {
                return new Symbol { Names = new List<string> { token.Lexeme } };
            }
            else
            {
                List<string> symbols = new List<string>();

                var currentToken = token;
                Token separator;
                do
                {
                    symbols.Add(currentToken.Lexeme);
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

                } while (separator.Type == TokenTypes.Dot && currentToken.Type == TokenTypes.TTWord);
                symbols.Reverse();
                return new Symbol { Names = symbols };
            }
        } 

        public static DataType GetDataTypeForToken(Token token, List<Token> tokens)
        {
            if (MochjScriptingService.Env == null) return null;
            Symbol symbol = GetFullSymbolFromTokenStream(token, tokens);
            return null;
        }

    }
}
