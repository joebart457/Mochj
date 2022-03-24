using Mochj._Parser.Models.Expressions;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer;
using Mochj.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


namespace Mochj._Parser
{
    public class ItemizedParser
    {
        private Parser _parser = new Parser();
        private Tokenizer _tokenizer = DefaultTokenizerBuilder.Build();


        public Ty ParseStatement<Ty>(string text) where Ty: Statement
        {
            var statements = _parser.Parse(_tokenizer.Tokenize(text));
            if (statements.Count() != 1) throw new Exception($"expected 1 statement of type {typeof(Ty).FullName} but got {statements.Count()}");
            Statement stmt = statements.First();
            if (stmt is Ty tyStmt)
            {
                return tyStmt;
            }
            throw new Exception($"expected to parse statement of type {typeof(Ty).FullName} but instead parsed statement of type {stmt.GetType().FullName}");
        }

        public Ty ParseStatementOrNull<Ty>(string text) where Ty : Statement
        {
            var statements = _parser.Parse(_tokenizer.Tokenize(text));
            if (statements.Count() != 1) return null;
            Statement stmt = statements.First();
            if (stmt is Ty tyStmt)
            {
                return tyStmt;
            }
            return null;
        }

        public Ty ParseExpression<Ty>(string text) where Ty : Expression
        {
            StmtExpression stmt = ParseStatementOrNull<StmtExpression>(text);
            if (stmt == null)
            {
                throw new Exception("unable to parse expression statement");
            }
            if (stmt.Expression is Ty expr)
            {
                return expr;
            }
            throw new Exception($"expected to parse expression of type {typeof(Ty).FullName} but instead parsed expression of type {stmt.Expression.GetType().FullName}");
        }

        public Ty ParseExpressionOrNull<Ty>(string text) where Ty : Expression
        {
            StmtExpression stmt = ParseStatementOrNull<StmtExpression>(text);
            if (stmt == null) return null;
            if (stmt.Expression is Ty expr)
            {
                return expr;
            }
            return null;
        }
       
    }
}
