using Microsoft.VisualStudio.Text;
using Mochj.IDE._Parser.Models.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = Mochj._Storage.Environment;


namespace Mochj.IDE._Interpreter.Models
{
    public class RangedStatementWithEnv
    {
        public Environment Environment { get; set; }
        public RangedStatement Statement { get; set; }

        public bool Contains(SnapshotPoint point)
        {
            if (Statement == null || Statement.TextRange == null) return false;
            return Statement.TextRange.Contains(point);
        }

        public RangedStatementWithEnv GetInnerMostContainedSpan(SnapshotPoint point)
        {
            if (!Contains(point)) return null;
            var match = this;
            var previousMatch = this;
            do
            {
                previousMatch = match;
                match = match.ChildSpans.Where(span => span.Contains(point)).FirstOrDefault();
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
