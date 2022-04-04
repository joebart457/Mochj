using Mochj._Tokenizer.Constants;
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
        public static List<RangedToken> TagFromPosition(TextPointer position)
        {
            var tokenizer = MochjScriptTokenizerBuilder.Build();
            List<RangedToken> rangedTokens = new List<RangedToken>();

            //what makes this work is checking the PointerContext so you avoid the markup characters   
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    var tokens = tokenizer.Tokenize(textRun).Where(t => t.Type != TokenTypes.EOF);
                    foreach (var token in tokens)
                    {
                        TextRange range = new TextRange(position.GetPositionAtOffset((int)token.Loc.X - token.Lexeme.Length), position.GetPositionAtOffset((int)token.Loc.X));
                        rangedTokens.Add(new RangedToken { Token = token, TextRange = range });
                    }
                }
                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            return rangedTokens;
        }
    }
}
