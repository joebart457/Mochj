using Mochj._Parser;
using Mochj._Parser.Models;
using Mochj._Parser.Models.Statements;
using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.IDE._Interpreter;
using Mochj.IDE._Interpreter.Models;
using Mochj.IDE._Parser;
using Mochj.IDE._Tokenizer;
using Mochj.IDE._Tokenizer.Models;
using Mochj.IDE.Builders;
using Mochj.Models;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Environment = Mochj._Storage.Environment;
namespace Mochj.IDE.Services
{
    public static class MochjScriptingService
    {
        public class ScriptExcecutionInfo
        {
            public ExecutionInfo ExecutionInfo { get; set; }
            public List<RangedToken> Tokens { get; set; }
        }
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
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }


        public static ScriptExcecutionInfo Run(TextPointer pointer)
        {
            var tokens = RangedTokenFactory.TagFromPosition(pointer);
            var stmts = new InformativeParser().Parse(tokens, out _);
            var interpreter = new InformedInterpreter(DefaultEnvironmentBuilder.Build(true));
            var info = interpreter.Interpret(stmts.ToList());
            return new ScriptExcecutionInfo { ExecutionInfo = info, Tokens = tokens };
        }

    }
}
