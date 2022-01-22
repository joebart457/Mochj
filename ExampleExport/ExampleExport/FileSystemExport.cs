using Mochj.Builders;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack;
namespace ExportItems
{
    public class Export
    {
        public int Setup(Mochj._Storage.Environment environment)
        {
            Mochj._Storage.Environment fsNamespace = new Mochj._Storage.Environment(null);
            fsNamespace.Define("foreach-in-dir",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      string pattern = args.Get<string>("pattern");
                      Function fn = args.Get<Function>("fn");
                      foreach (string file in Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories))
                      {
                          IList<Argument> arguments = new List<Argument>();
                          arguments.Add(new Argument { Position = 0, Value = QualifiedObjectBuilder.BuildString(file) });
                          fn.Call(fn.ResolveArguments(arguments));
                      }

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<Function>("fn")
                  .RegisterParameter<string>("pattern", QualifiedObjectBuilder.BuildString("*.*"))
                  .ReturnsEmpty()
                  .Build()
              ));

            environment.Define("FileSystem", QualifiedObjectBuilder.BuildNamespace(fsNamespace));

            return 0;
        }
    }
}
