using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Tokenizer.Models
{
	public class TokenizerRule
	{
		private int _length;
		public int Length { get { return _length; } }
		private string _value;
		public string Value { get { return _value; } set { _value = value; _length = _value.Length; } }
		public string Type { get; set; }
		public string Macro { get; set; }
		public TokenizerRule(string type, string value, string macro = null)
		{
			Type = type;
			Value = value;
			Macro = macro;
		}

		public string GetValueOrMacro()
		{
			if (Macro == null)
			{
				return Value;
			}
			return Macro;
		}
	}
}
