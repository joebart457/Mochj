using Mochj._Tokenizer.Models;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace Mochj.Services
{
    static class ProgramStartupService
    {
        private static readonly string StartupDirectoryVariable = "Directory";
        private static readonly string RunFileVariable = "RunFile";

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
                    SetWorkingDirectoryAndRunFile(null);
                    ShowHelp();
                    repl();
                    return 0;
                }

                string runFile = TypeMediatorService.ToNativeType<string>(arguments.Get(0));
                SetWorkingDirectoryAndRunFile(runFile);

                var Tokenizer = DefaultTokenizerBuilder.Build();
                var Parser = new _Parser.Parser();
                var Interpreter = new _Interpreter.Interpreter(DefaultEnvironmentBuilder.Default);

                Interpreter.Accept(Parser.Parse(Tokenizer.TokenizeFile(runFile)));
                Function entryFn = Interpreter.GetEntryPoint();
                if (entryFn != null) entryFn.Call(entryFn.ResolveArguments(arguments.ToList(0)));
                return 0;
            }
            catch(ExitException ee)
            { 
                Environment.ExitCode = ee.Value;
                return ee.Value;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        private static void SetWorkingDirectoryAndRunFile(string runFile)
        {
            _Storage.Environment startupNamespace = new _Storage.Environment(null).WithAlias("Startup");

            string fullPath = System.Reflection.Assembly.GetCallingAssembly().Location;
            string workingDirectory = Path.GetDirectoryName(fullPath);

            if (!string.IsNullOrWhiteSpace(runFile))
            {
                fullPath = Path.GetFullPath(runFile);
                workingDirectory = Path.GetDirectoryName(fullPath);
            }

            startupNamespace.Define(StartupDirectoryVariable, QualifiedObjectBuilder.BuildString(workingDirectory));
            startupNamespace.Define(RunFileVariable, QualifiedObjectBuilder.BuildString(fullPath));

            DefaultEnvironmentBuilder.Default.Define("Startup", QualifiedObjectBuilder.BuildNamespace(startupNamespace));

            Directory.SetCurrentDirectory(workingDirectory);
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
                catch(ExitException ee)
                {
                    Console.WriteLine(ee.Message);
                    Environment.ExitCode = ee.Value;
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }


    }
}
