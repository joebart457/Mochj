using Mochj.IDE._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.IDE._Parser.Helpers
{
    public class ParsingHelper
    {
        private int _index = 0;
        private bool _bAtEnd = false;
        private RangedToken _current = null;
        private IEnumerable<RangedToken> _tokens;

        protected bool init(IEnumerable<RangedToken> tokens)
        {
            reset();
            _tokens = tokens;
            if (tokens.Count() == 0)
            {
                _bAtEnd = true;
                return false;
            }
            _current = tokens.First();
            return true;
        }
        protected void reset()
        {
            _index = 0;
            _bAtEnd = false;
            _current = null;
        }

        protected void advance()
        {
            if (_bAtEnd)
            {
                throw new IndexOutOfRangeException("Cannot advance past end");
            }
            _index++;
            if (_index >= _tokens.Count())
            {
                _bAtEnd = true;
                return;
            }
            _current = _tokens.ElementAt(_index);
        }

        protected bool match(string type)
        {
            if (_bAtEnd || _current == null)
            {
                return false;
            }
            if (_current.Token.Type == type)
            {
                advance();
                return true;
            }
            return false;
        }
        protected bool match(RangedToken token, string type)
        {
            if (token.Token.Type == type)
            {
                return true;
            }
            return false;
        }

        protected bool lexMatch(string lexeme)
        {
            if (_bAtEnd || _current == null)
            {
                return false;
            }
            if (_current.Token.Lexeme == lexeme)
            {
                advance();
                return true;
            }
            return false;
        }

        protected RangedToken previous()
        {
            if (_index - 1 >= _tokens.Count())
            {
                throw new IndexOutOfRangeException("failed getting previous token");
            }
            return _tokens.ElementAt(_index - 1);
        }

        protected RangedToken consume(string type, string errorMessage = "")
        {
            if (_bAtEnd)
            {
                throw new IndexOutOfRangeException($"at end when calling consume; {errorMessage}");
            }
            if (match(type))
            {
                return previous();
            }
            throw new ArgumentException($"{errorMessage} at {(!_bAtEnd? current().Token.ToString(): "null")}");
        }

        protected bool peekMatch(int offset, string type)
        {
            if (_index + offset < _tokens.Count())
            {
                return _tokens.ElementAt(_index + offset).Token.Type == type;
            }
            return false;
        }

        protected bool atEnd()
        {
            return _bAtEnd;
        }

        protected RangedToken current()
        {
            return _current;
        }
    }
}
