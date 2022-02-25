using Mochj.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mochj._Interpreter.Helpers
{
    public static class LoadFileHelper
    {
        public static string ExecutableDirectory()
        {
            string strExeFilePath = Assembly.GetCallingAssembly().Location;
            return System.IO.Path.GetDirectoryName(strExeFilePath);
        }
        /// <summary>
        /// Takes a filepath, strips file name 
        /// and returns new full path to the same file name in the 
        /// directory where this executable resides
        /// </summary>
        /// <returns></returns>
        public static string SwitchPathToExecutableHome(string path)
        {
            string filename = System.IO.Path.GetFileName(path);
            if (string.IsNullOrEmpty(filename)) return null;
            return $"{ExecutableDirectory()}\\pkg\\{filename}";
        }
        public static void LoadFromAssembly(_Storage.Environment environment, string path)
        {
            try
            {
                var DLL = Assembly.LoadFrom(path);
                var type = DLL.GetType("ExportItems.Export");
                var targetObject = Activator.CreateInstance(type);
                int rc = (int)type.InvokeMember("Setup", BindingFlags.InvokeMethod, null, targetObject, new object[] { environment });
                if (rc != 0)
                {
                    throw new Exception($"loading compiled library at {path} was successful, but Setup returned with code {rc}");
                }
            } catch(Exception e)
            {
                throw new Exception($"error loading compiled library at {path}: {e}");
            }
        }

        public static void LoadFromRawCode(_Storage.Environment environment, string path)
        {
            Interpreter interpreter = new Interpreter(environment);
            _Parser.Parser parser = new _Parser.Parser();
            _Tokenizer.Tokenizer tokenizer = DefaultTokenizerBuilder.Build();

            try
            {
                interpreter.Accept(parser.Parse(tokenizer.TokenizeFile(path)));
            }
            catch (Exception e)
            {
                throw new Exception($"error while loading {path}: {e.Message}");
            }
        }

    }
}
