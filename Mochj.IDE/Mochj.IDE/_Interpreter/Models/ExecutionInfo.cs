using Mochj.IDE._Parser.Models.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Environment = Mochj._Storage.Environment;


namespace Mochj.IDE._Interpreter.Models
{
    public class RangedStatementWithEnv
    {
        public Environment Environment { get; set; }
        public RangedStatement Statement { get; set; }

        public bool Contains(TextPointer pointer)
        {
            if (Statement == null || Statement.TextRange == null) return false;
            return Statement.TextRange.Contains(pointer);
        }

        public RangedStatementWithEnv GetInnerMostContainedSpan(TextPointer pointer)
        {
            if (!Contains(pointer)) return null;
            var match = this;
            var previousMatch = this;
            do
            {
                previousMatch = match;
                match = match.ChildSpans.Where(span => span.Contains(pointer)).FirstOrDefault();
            } while (match != null);

            return previousMatch;
        }

        public List<RangedStatementWithEnv> ChildSpans { get; set; } = new List<RangedStatementWithEnv>();
    }
    public class ExecutionInfo
    {
        public Environment Environment { get; set; }
        public List<RangedStatementWithEnv> Spans { get; set; }
    }
}
