﻿using Microsoft.WindowsAPICodePack.Shell;
using Mochj.Builders;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExportItems
{
    public class Export
    {
        public int Setup(Mochj._Storage.Environment environment)
        {
            Mochj._Storage.Environment fsNamespace = new Mochj._Storage.Environment(null);
            Mochj._Storage.Environment fileNamespace = new Mochj._Storage.Environment(null);
            Mochj._Storage.Environment dirNamespace = new Mochj._Storage.Environment(null);

            fsNamespace.Define("File", QualifiedObjectBuilder.BuildNamespace(fileNamespace));
            fsNamespace.Define("Dir", QualifiedObjectBuilder.BuildNamespace(dirNamespace));

            dirNamespace.Define("foreach",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            dirNamespace.Define("exists",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildBoolean(Directory.Exists(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<bool>()
                  .Build()
              ));

            dirNamespace.Define("create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            dirNamespace.Define("delete",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("foreach-tag",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("exists",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildBoolean(File.Exists(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<bool>()
                  .Build()
              ));

            fileNamespace.Define("create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("move",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("copy",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("delete",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("write",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("read",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");

                      return QualifiedObjectBuilder.BuildString(File.ReadAllText(path));
                  })
                  .RegisterParameter<string>("path")
                  .RegisterParameter<string>("data")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("foreach-line",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
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

            fileNamespace.Define("get-filename",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFileName(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("get-directory",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetDirectoryName(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("get-ext",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetExtension(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("get-filename-without-ext",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFileNameWithoutExtension(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fileNamespace.Define("get-fullpath",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string path = args.Get<string>("path");
                      return QualifiedObjectBuilder.BuildString(Path.GetFullPath(path));
                  })
                  .RegisterParameter<string>("path")
                  .Returns<string>()
                  .Build()
              ));

            fsNamespace.Define("exe-home",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                      return QualifiedObjectBuilder.BuildString(Path.GetDirectoryName(strExeFilePath));
                  })
                  .Returns<string>()
                  .Build()
              ));

            fsNamespace.Define("exe-path",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
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
