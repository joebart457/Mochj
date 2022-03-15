using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Tokenizer.Constants
{
    static class TokenTypes
    {
        // Tokenizer required types
        public const string StringEnclosing = "StringEnclosing";
        public const string StringCatalyst = "StringCatalyst";
        public const string EOLComment = "EOLComment";
        public const string MLCommentStart = "MLCommentStart";
        public const string MLCommentEnd = "MLCommentEnd";
        public const string EOF = "EOF";
        public const string TTString = "TTString";
        public const string TTWord = "TTWord";
        public const string TTInteger = "TTInteger";
        public const string TTUnsignedInteger = "TTUnsignedInteger";
        public const string TTFloat = "TTFloat";
        public const string TTDouble = "TTDouble";

        // Custom types - words
        public const string Defn = "Defn";
        public const string Set = "Set";
        public const string Namespace = "Namespace";
        public const string Load = "Load";
        public const string Entry = "Entry";

        public const string LiteralTrue = "LiteralTrue";
        public const string LiteralFalse = "LiteralFalse";

        public const string Number = "Number";
        public const string Boolean = "Boolean";
        public const string NativeList = "NativeList";
        public const string String = "String";
        public const string Fn = "Fn";
        public const string Empty = "Empty";
        public const string Any = "Any";

        // Custom types - single character symbols
        public const string SemiColon = "SemiColon";
        public const string Comma = "Comma";
        public const string Not = "Not";
        public const string Dot = "Dot";
        public const string Ampersand = "Ampersand";
        public const string Pipe = "Pipe";
        public const string Plus = "Plus";
        public const string Dash = "Dash";
        public const string Asterisk = "Asterisk";
        public const string ForwardSlash = "ForwardSlash";
        public const string Equal = "Equal";
        public const string LParen = "LParen";
        public const string RParen = "RParen";
        public const string LBracket = "LBracket";
        public const string RBracket = "RBracket";
        public const string LCurly = "LCurly";
        public const string RCurly = "RCurly";
        public const string LCarat = "LCarat";
        public const string RCarat = "RCarat";
        public const string UpCarat = "UpCarat";
        public const string Colon = "Colon";

        // Custom types - multiple character symbols
        public const string And = "And";
        public const string Or = "Or";
        public const string EqualEqual = "EqualEqual";
        public const string NotEqual = "NotEqual";
        public const string LessEqual = "LessEqual";
        public const string GreaterEqual = "GreaterEqual";

        public const string DoubleLCarat = "DoubleLCarat";
        public const string DoubleRCarat = "DoubleRCarat";
        public const string DoubleDash = "DoubleDash";
    }
}
