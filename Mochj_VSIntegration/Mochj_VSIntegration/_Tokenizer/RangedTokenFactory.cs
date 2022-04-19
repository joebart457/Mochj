using Microsoft.VisualStudio.Text;
using Mochj._Tokenizer.Constants;
using Mochj._Tokenizer.Models;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Mochj.IDE._Tokenizer
{
    static class RangedTokenFactory
    {
        public static List<RangedToken> TokenizeFromSnapshot(ITextSnapshot snapshot)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            List<RangedToken> rangedTokens = new List<RangedToken>();

            if (snapshot != null)
            {
                var tokens = tokenizer.Tokenize(snapshot.GetText()).Where(t => t.Type != TokenTypes.EOF);
                foreach (var token in tokens)
                { 
                    rangedTokens.Add(new RangedToken { Token = token, TextRange = GetSpanForToken(token) });
                }
            }

            return rangedTokens;
        }

        private static Span GetSpanForToken(Token token)
        {
            int frontOffset = 0;
            int backOffset = 0;
            if (!string.IsNullOrEmpty(token.EnclosingFront))
            {
                frontOffset = token.EnclosingFront.Length;
            }
            if (!string.IsNullOrEmpty(token.EnclosingBack))
            {
                backOffset = token.EnclosingBack.Length;
            }
            return new Span((int)token.Loc.X - token.Lexeme.Length - frontOffset - backOffset, token.Lexeme.Length + backOffset + frontOffset);
        }
    }
}
