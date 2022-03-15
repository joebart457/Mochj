using Mochj._PackageManager.Models;
using Mochj._PackageManager.Models.Constants;
using Mochj.Builders;
using Mochj.Models.Constants;
using System;
using System.Collections.Generic;
using System.IO;
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
            return Path.GetDirectoryName(strExeFilePath);
        }
        /// <summary>
        /// Takes a filepath, strips file name 
        /// and returns new full path to the same file name in the 
        /// directory where this executable resides
        /// </summary>
        /// <returns></returns>
        public static string SwitchPathToExecutableHome(string path)
        {
            string filename = Path.GetFileName(path);
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

        public static void LoadFile(_Storage.Environment environment, string path)
        {
            if (Path.GetExtension(path).ToLower() == ".dll")
            {
                LoadFromAssembly(environment, path);
            } else
            {
                LoadFromRawCode(environment, path);
            }
        }

        public static void LoadPackage(_Storage.Environment environment, string packageName, string version)
        {
            RemotePackage pkg = _PackageManager.PackageManager.FindPackage(packageName, version, DefaultPathConstants.ManifestPath);
            string pkgDir = $"{PathConstants.PackagePath}{packageName}@{version}/";
            foreach(string file in pkg.LoadFiles)
            {
                LoadFile(environment, Path.Combine(pkgDir, file));
            }
        }

    }
}
