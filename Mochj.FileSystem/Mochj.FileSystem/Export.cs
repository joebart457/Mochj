using Microsoft.WindowsAPICodePack.Shell;
using Mochj.Builders;
using Mochj.Models;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace ExportItems
{
    public class Export
    {
        public int Setup(Mochj._Storage.Environment environment)
        {
            Mochj._Storage.Environment fsNamespace = new Mochj._Storage.Environment(null).WithAlias("FileSystem");
            Mochj._Storage.Environment fileNamespace = new Mochj._Storage.Environment(null).WithAlias("File");
            Mochj._Storage.Environment dirNamespace = new Mochj._Storage.Environment(null).WithAlias("Directory");

            fsNamespace.Define("File", QualifiedObjectBuilder.BuildNamespace(fileNamespace));
            fsNamespace.Define("Directory", QualifiedObjectBuilder.BuildNamespace(dirNamespace));

            dirNamespace.Define("ForEach",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ForEach")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      string pattern = args.Get<string>("pattern");
                      Function fn = args.Get<Function>("fn");
                      bool recursive = args.Get<bool>("recursive");

                      foreach (string file in Directory.EnumerateFiles(path, pattern, recursive? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
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
                  .RegisterParameter<bool>("recursive", QualifiedObjectBuilder.BuildBoolean(false))
                  .ReturnsEmpty()
                  .Build()
              ));

            dirNamespace.Define("Exists",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Exists")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildBoolean(Directory.Exists(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<bool>()
                  .Build()
              ));

            dirNamespace.Define("Create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Create")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      Directory.CreateDirectory(path);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .ReturnsEmpty()
                  .Build()
              ));

            dirNamespace.Define("Delete",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Delete")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      bool recursive = args.Get<bool>("recursive");

                      Directory.Delete(path, recursive);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<bool>("recursive", QualifiedObjectBuilder.BuildBoolean(false))
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("ForEachTag",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ForEachTag")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      Function fn = args.Get<Function>("fn");

                      ShellFile shellFile = ShellFile.FromFilePath(path);

                      string[] tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject ?? new string[0];

                      if (tags.Length != 0)
                      {
                          foreach (string tag in tags)
                          {
                              IList<Argument> arguments = new List<Argument>();
                              arguments.Add(new Argument { Position = 0, Value = QualifiedObjectBuilder.BuildString(tag) }); 

                              fn.Call(fn.ResolveArguments(arguments));
                          }
                      }

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<Function>("fn")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("GetTags",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetTags")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      ShellFile shellFile = ShellFile.FromFilePath(path);

                      string[] tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject ?? new string[0];

                      return QualifiedObjectBuilder.BuildNativeList(tags.Select(value => QualifiedObjectBuilder.BuildString(value)).ToList());
                  })
                  .RegisterParameter<string>("path")
                  .Returns<NativeList>()
                  .Build()
              ));

            fileNamespace.Define("Exists",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Exists")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildBoolean(File.Exists(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<bool>()
                  .Build()
              ));

            fileNamespace.Define("Create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Create")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      File.Create(path);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("Move",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Move")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>("source");
                      string destination = args.Get<string>("destination");

                      File.Move(source, destination);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("destination")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("Copy",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Copy")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>("source");
                      string destination = args.Get<string>("destination");
                      bool overwrite = args.Get<bool>("overwrite");

                      File.Copy(source, destination, overwrite);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("destination")
                  .RegisterParameter<bool>("overwrite", QualifiedObjectBuilder.BuildBoolean(true))
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("Delete",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Delete")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      File.Delete(path);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("Write",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Write")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      string data = args.Get<string>("data");

                      File.WriteAllText(path, data);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<string>("data")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("Append",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Append")
                    .Action((Args args) =>
                    {
                        string path = args.Get<string>("path");
                        string data = args.Get<string>("data");

                        File.AppendAllText(path, data);

                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<string>("path")
                    .RegisterParameter<string>("data")
                    .ReturnsEmpty()
                    .Build()
                ));

            fileNamespace.Define("Read",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Read")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildString(File.ReadAllText(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("ReadLines",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ReadLines")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      string[] lines = File.ReadAllLines(path);
                      return QualifiedObjectBuilder.BuildNativeList(lines.Select(value => QualifiedObjectBuilder.BuildString(value)).ToList());
                  })
                  .RegisterParameter<string>("path")
                  .Returns<NativeList>()
                  .Build()
              ));

            fileNamespace.Define("ForEachLine",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ForEachLine")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      Function fn = args.Get<Function>("fn");
                      foreach(string line in File.ReadLines(path))
                      {
                          IList<Argument> arguments = new List<Argument>();
                          arguments.Add(new Argument { Position = 0, Value = QualifiedObjectBuilder.BuildString(line) });

                          fn.Call(fn.ResolveArguments(arguments));
                      }

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<Function>("fn")
                  .ReturnsEmpty()
                  .Build()
              ));

            fileNamespace.Define("GetFileName",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetFileName")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFileName(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("GetDirectory",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetDirectory")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetDirectoryName(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("GetExtension",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetExtension")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetExtension(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("GetFileNameWithoutExtension",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetFileNameWithoutExtension")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFileNameWithoutExtension(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("GetFullPath",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetFullPath")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFullPath(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("GenerateHash",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GenerateHash")
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>(0);
                      using (var md5 = MD5.Create())
                      {
                          using (var stream = File.OpenRead(path))
                          {
                              var hash = md5.ComputeHash(stream);
                              return QualifiedObjectBuilder.BuildString(BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant());
                          }
                      }
                      
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fsNamespace.Define("GetExeHome",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetExeHome")
                  .Action((Args args) =>
                  {
                      string strExeFilePath = System.Reflection.Assembly.GetCallingAssembly().Location;
                      return QualifiedObjectBuilder.BuildString(Path.GetDirectoryName(strExeFilePath));
                  })
                  .Returns<string>()
                  .Build()
              ));

            fsNamespace.Define("GetExePath",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("GetExePath")
                  .Action((Args args) =>
                  {
                      string strExeFilePath = System.Reflection.Assembly.GetCallingAssembly().Location;
                      return QualifiedObjectBuilder.BuildString(strExeFilePath);
                  })
                  .Returns<string>()
                  .Build()
              ));

            environment.Define("FileSystem", QualifiedObjectBuilder.BuildNamespace(fsNamespace));

            return 0;
        }
    }
}
