using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Tokenizer.Models
{
    public class Token
    {
        public Location Loc { get; set; }
        public string Lexeme { get; set; }
        public string Type { get; set; }

        public Token(TokenizerRule rule, uint nRow, uint nColumn)
        {
            Lexeme = rule.Value;
            Type = rule.Type;
            Loc = new Location(nColumn, nRow);
        }

        public Token(string type, string lexeme, uint nRow, uint nColumn)
        {
            Lexeme = lexeme;
            Type = type;
            Loc = new Location(nColumn, nRow);
        }

        public override string ToString()
        {
            return $"Token({Type}|{Lexeme}) at {Loc.ToString()}";
        }
    }
}
