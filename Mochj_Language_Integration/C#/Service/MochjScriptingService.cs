using Mochj._Interpreter;
using Mochj._Parser;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Services;
using MochjLanguage._Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = Mochj._Storage.Environment;


namespace MochjLanguage.Service
{
    public static class MochjScriptingService
    {

        public static Environment Env { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Must be partial symbol with no base name</param>
        /// <returns></returns>
        public static List<string> GetEnvMembersForSymbol(Symbol symbol)
        {
            if (Env == null) { return new List<string>(); }
            if (symbol == null || symbol.Names == null || !symbol.Names.Any())
            {
                return Env.Lookup.Select(kv => kv.Key).ToList();
            }

            try
            {
                var curEnv = Env;
                QualifiedObject intermObj = null;
                foreach (string name in symbol.Names)
                {
                    if (Env.TryGet(name, out intermObj))
                    {
                        if (intermObj.Type.Is(Mochj.Enums.DataTypeEnum.Namespace))
                        {
                            curEnv = TypeMediatorService.ToNativeType<Environment>(intermObj);
                        }
                    }
                    else
                    {
                        return new List<string>();
                    }
                }

                return curEnv.Lookup.Select(kv => kv.Key).ToList();
            } catch (Exception)
            {
                return new List<string>();
            }

        }

        public static bool RunScriptInBackground(string rawText)
        {
            try
            {
                var env = DefaultEnvironmentBuilder.Build(true);
                var interpreter = new MochjScriptInterpreter(env);
                var stmts = ParseTokens(TokenizeText(rawText), out var invalidToken);
                foreach(Statement stmt in stmts)
                {
                    if (stmt is StmtExpression expr) { }
                    else if (stmt != null)
                    {
                        stmt.Visit(interpreter);
                    }
                }
                Env = env;
                return true;
            } catch(Exception)
            {
                return false;
            }
        }
        public static IEnumerable<Statement> ParseTokens(IEnumerable<Token> tokens, out Token invalidToken)
        {
            var parser = new ResilientParser();
            return parser.Parse(tokens, out invalidToken);
        }

        public static IEnumerable<Token> TokenizeText(string rawText)
        {
            return DefaultTokenizerBuilder.Build().Tokenize(rawText);
        }
    }
}
