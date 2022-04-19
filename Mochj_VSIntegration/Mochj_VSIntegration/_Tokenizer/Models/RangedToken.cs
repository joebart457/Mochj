using Mochj._Tokenizer.Models;
using Mochj.IDE.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Environment = Mochj._Storage.Environment;

namespace Mochj.IDE._Tokenizer.Models
{
    public class RangedToken
    {
        public Token Token { get; set; }
        public Microsoft.VisualStudio.Text.Span TextRange { get; set; }
        public Environment Environment { get; set; }
        public TokenClassifierEnum Classifier { get; set; } = TokenClassifierEnum.Unkown;
        public string Message { get; set; }

        public override string ToString()
        {
            return Token.ToString();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals (this, null)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is RangedToken other)
                return Token.Equals(other.Token) && TextRange.Equals(other.TextRange) && Environment.Equals(other.Environment) && Classifier == other.Classifier && Message == other.Message;
            return false;
        }

        public override int GetHashCode()
        {
            return Classifier.GetHashCode() + (Message == null? 0: Message.GetHashCode()) + Token.GetHashCode();
        }

    }
}
