﻿using Mochj._Interpreter;
using Mochj._Tokenizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Parser.Models.Statements
{
    class StmtUse : Statement
    {
        public string Name { get; set; }
        public StmtUse(Location loc)
            : base("StmtUse", loc)
        {

        }

        public override void Visit(Interpreter interpreter)
        {
            interpreter.Accept(this);
        }
    }
}
