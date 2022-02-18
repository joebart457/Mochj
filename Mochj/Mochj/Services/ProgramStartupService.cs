using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Services
{
    static class ProgramStartupService
    {
       
        private static void ShowHelp()
        {
            Console.WriteLine("usage: mochj.exe [run_file] [args...]");
            Console.WriteLine("      option      |     desc    ");
            Console.WriteLine("     run_file     | script file to run ");
            Console.WriteLine("     [args...]    | any number of arguments");
            Console.WriteLine("                  | to be passed to program entry point ");
            Console.WriteLine("                  | arguments can be expressed as either ");
            Console.WriteLine("                  | positional (ie. <value1> <value2> ...) ");
            Console.WriteLine("                  | or verbose(ie.  --argname <value> ) ");
        }

        public static int Startup(string[] args)
        {
            try
            {
                Args arguments = new _CmdParser.CmdParser().ParseCommandLineArgs(args);
                if (!arguments.ToList().Any())
                {
                    ShowHelp();
                    repl();
                    return 0;
                }

                string runFile = TypeMediatorService.ToNativeType<string>(arguments.Get(0));

                var Tokenizer = DefaultTokenizerBuilder.Build();
                var Parser = new _Parser.Parser();
                var Interpreter = new _Interpreter.Interpreter(DefaultEnvironmentBuilder.Default);

                Interpreter.Accept(Parser.Parse(Tokenizer.TokenizeFile(runFile)));
                Function entryFn = Interpreter.GetEntryPoint();
                entryFn.Call(entryFn.ResolveArguments(arguments.ToList(0)));
                return 0;
            } catch(Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public static void repl()
        {
            var Tokenizer = DefaultTokenizerBuilder.Build();
            var Parser = new _Parser.Parser();
            var Interpreter = new _Interpreter.Interpreter(DefaultEnvironmentBuilder.Default);


            bool bDebug = false;

            while (true)
            {
                Console.Write("% ");
                string input = Console.ReadLine();

                if (input.Trim() == "exit")
                {
                    break;
                }

                if (input.Trim() == "-debug")
                {
                    bDebug = !bDebug;
                    continue;
                }

                try
                {
                    Interpreter.Accept(Parser.Parse(Tokenizer.Tokenize(input, bDebug)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


    }
}
