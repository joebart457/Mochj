using Mochj.Models;
using Mochj.Models.ControlFlow;
using Mochj.Models.Fn;
using Mochj.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mochj.Builders
{
    public static class DefaultEnvironmentBuilder
    {
        private static _Storage.Environment _default = null;
        public static _Storage.Environment Default { get { return _default == null ? Build() : _default; } }
        public static _Storage.Environment Build()
        {
            if (_default != null) return _default;
            _Storage.Environment environment = new _Storage.Environment(null);
            _default = environment;

            _Storage.Environment listNamespace = new _Storage.Environment(null);
            _Storage.Environment pmNamespace = new _Storage.Environment(null);
            _Storage.Environment nativeListNamespace = new _Storage.Environment(null);

            environment.Define("version", QualifiedObjectBuilder.BuildString("2.00"));

            environment.Define("typeof",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        QualifiedObject obj = args.Get(0);

                        return QualifiedObjectBuilder.BuildTypeInfo(obj.Type);
                    })
                    .RegisterParameter<object>("any")
                    .Returns<DataType>()
                    .Build()
                ));

            #region TypeInitializations

            environment.Define("Empty", QualifiedObjectBuilder.EmptyFunction());

            environment.Define("Number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0)); })
                    .RegisterParameter<object>("convertibleObject")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("String",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (QualifiedObjectBuilder.BuildEmptyValue().Equals(args.Get(0)))
                        {
                            return QualifiedObjectBuilder.BuildString("empty");
                        }
                        return QualifiedObjectBuilder.BuildString(args.Get<string>(0));
                    }
                    )
                    .RegisterParameter<object>("convertibleObject")
                    .Returns<string>()
                    .Build()
                ));

            environment.Define("Boolean",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildBoolean(args.Get<bool>(0)); })
                    .RegisterParameter<object>("convertibleObject")
                    .Returns<bool>()
                    .Build()
                ));

            #endregion

            #region BasicOps

            environment.Define("add-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("addn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("sub-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) - args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("subn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) - args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("mul-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("muln",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));


            environment.Define("div-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) / args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("divn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) / args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));


            environment.Define("add-string",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        StringBuilder result = new StringBuilder(args.Get<string>(0));

                        var remainingArgs = args.ToList(0);
                        foreach (Argument argument in remainingArgs)
                        {
                            result.Append(TypeMediatorService.ToNativeType<string>(argument.Value));
                        }
                        return QualifiedObjectBuilder.BuildString(result.ToString());
                    })
                    .RegisterParameter<string>("a")
                    .VariadicAfter(0)
                    .VariadicType<string>()
                    .Returns<string>()
                    .Build()
                ));

            environment.Define("adds",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        StringBuilder result = new StringBuilder(args.Get<string>(0));

                        var remainingArgs = args.ToList(0);
                        foreach (Argument argument in remainingArgs)
                        {
                            result.Append(TypeMediatorService.ToNativeType<string>(argument.Value));
                        }
                        return QualifiedObjectBuilder.BuildString(result.ToString());
                    })
                    .RegisterParameter<string>("a")
                    .VariadicAfter(0)
                    .VariadicType<string>()
                    .Returns<string>()
                    .Build()
                ));

            #endregion

            environment.Define("println",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { Console.WriteLine(args.Get<object>(0)); return QualifiedObjectBuilder.BuildEmptyValue(); })
                    .RegisterParameter<object>("a")
                    .ReturnsEmpty()
                    .Build()
                ));

            environment.Define("return",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { throw new ReturnException(args.Get(0)); })
                    .RegisterParameter<object>("obj")
                    .Returns<object>()
                    .Build()
                ));

            environment.Define("ToString",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildString(args.Get<object>(0).ToString()); })
                    .RegisterParameter<object>("obj")
                    .Returns<string>()
                    .Build()
                ));

            environment.Define("bind",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        Function fn = args.Get<Function>(0);
                        Function boundFn = new BoundFn
                        {
                            hFunc = fn,
                            BoundArguments = new Args(fn.Params.PatchArguments(args.ToList(0))),
                        };
                        return QualifiedObjectBuilder.BuildFunction(boundFn);
                    })
                    .RegisterParameter<Function>("fn")
                    .VariadicAfter(0)
                    .Returns<Function>()
                    .Build()
                ));

            environment.Define("get-arg",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        BoundFn fn = args.Get<BoundFn>(0);
                        return fn.BoundArguments.Get(args.Get<string>(1));
                    })
                    .RegisterParameter<BoundFn>("fn")
                    .RegisterParameter<string>("key")
                    .Returns<object>()
                    .Build()
                ));

            environment.Define("replace-arg",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        BoundFn fn = args.Get<BoundFn>(0);
                        fn.BoundArguments.Replace(args.Get<string>(1), args.Get(2));
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<BoundFn>("fn")
                    .RegisterParameter<string>("key")
                    .RegisterParameter<object>("newValue")
                    .ReturnsEmpty()
                    .Build()
                ));



            #region ControlFlow

            environment.Define("if",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       bool condition = args.Get<bool>(0);
                       Function doThis = args.Get<Function>(1);
                       Function elseDo = args.Get<Function>(2);

                       if (condition)
                       {
                           return doThis.Call(new Args());
                       }
                       else
                       {
                           return elseDo.Call(new Args());
                       }
                   })
                   .RegisterParameter<bool>("condition")
                   .RegisterParameter<Function>("doThis")
                   .RegisterParameter<Function>("elseDo")
                   .Returns<object>()
                   .Build()
               ));

            environment.Define("while",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       Function condition = args.Get<Function>(0);
                       Function doThis = args.Get<Function>(1);

                       while (TypeMediatorService.IsTruthy(condition.Call(new Args()).Object))
                       {
                           try
                           {
                               doThis.Call(new Args());
                           }
                           catch (BreakException)
                           {
                               break;
                           }
                           catch (ContinueException)
                           {
                               continue;
                           }
                       }
                       return QualifiedObjectBuilder.BuildEmptyValue();
                   })
                   .RegisterParameter<Function>("condition")
                   .RegisterParameter<Function>("doThis")
                   .ReturnsEmpty()
                   .Build()
               ));

            environment.Define("break",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       throw new BreakException();
                   })
                   .ReturnsEmpty()
                   .Build()
               ));

            environment.Define("continue",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       throw new ContinueException();
                   })
                   .ReturnsEmpty()
                   .Build()
               ));

            environment.Define("try",
                 QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (args.Get(0) == QualifiedObjectBuilder.EmptyFunction()) return QualifiedObjectBuilder.BuildBoolean(false);
                        BoundFn fn = args.Get<BoundFn>(0);

                        try
                        {
                            fn.Call(fn.ResolveArguments(new List<Argument>()));
                            return QualifiedObjectBuilder.BuildString(string.Empty);
                        }
                        catch (Exception e)
                        {
                            return QualifiedObjectBuilder.BuildString(e.Message);
                        }
                    })
                    .RegisterParameter<BoundFn>("fn")
                    .Returns<string>()
                    .Build()
            ));

            #endregion

            #region Comparators

            environment.Define("equal",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(args.Get(0).Equals(args.Get(1)));
                   })
                   .RegisterParameter<object>("a")
                   .RegisterParameter<object>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("equals",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(args.Get(0).Equals(args.Get(1)));
                   })
                   .RegisterParameter<object>("a")
                   .RegisterParameter<object>("b")
                   .Returns<bool>()
                   .Build()
               ));


            environment.Define("not-equal",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(!args.Get(0).Equals(args.Get(1)));
                   })
                   .RegisterParameter<object>("a")
                   .RegisterParameter<object>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("not-equals",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(!args.Get(0).Equals(args.Get(1)));
                   })
                   .RegisterParameter<object>("a")
                   .RegisterParameter<object>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("gt",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a > b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("gte",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a >= b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("lt",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a < b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("lte",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a <= b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("greater-than",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a > b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("greater-than-equal",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a >= b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("less-than",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a < b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("less-than-equal",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       double a = args.Get<double>(0);
                       double b = args.Get<double>(1);

                       return QualifiedObjectBuilder.BuildBoolean(a <= b);
                   })
                   .RegisterParameter<double>("a")
                   .RegisterParameter<double>("b")
                   .Returns<bool>()
                   .Build()
               ));

            #endregion

            #region LogicOps


            environment.Define("not",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(!args.Get<bool>(0));
                   })
                   .RegisterParameter<bool>("value")
                   .Returns<bool>()
                   .Build()
               ));

            environment.Define("and",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(args.Get<bool>(0) && args.Get<bool>(1));
                   })
                   .RegisterParameter<bool>("a")
                   .RegisterParameter<bool>("b")
                   .Returns<bool>()
                   .Build()
               ));


            environment.Define("or",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       return QualifiedObjectBuilder.BuildBoolean(args.Get<bool>(0) || args.Get<bool>(1));
                   })
                   .RegisterParameter<bool>("a")
                   .RegisterParameter<bool>("b")
                   .Returns<bool>()
                   .Build()
               ));

            #endregion

            environment.Define("success",
                 QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (args.Get(0) == QualifiedObjectBuilder.EmptyFunction()) return QualifiedObjectBuilder.BuildBoolean(false);
                        BoundFn fn = args.Get<BoundFn>(0);

                        try
                        {
                            fn.Call(fn.ResolveArguments(new List<Argument>()));
                            return QualifiedObjectBuilder.BuildBoolean(true);
                        }
                        catch (Exception)
                        {
                            return QualifiedObjectBuilder.BuildBoolean(false);
                        }
                    })
                    .RegisterParameter<BoundFn>("fn")
                    .Returns<bool>()
                    .Build()
            ));


            environment.Define("call",
                 QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        Function fn = args.Get<Function>(0);
                        return fn.Call(fn.ResolveArguments(args.ToList(0)));

                    })
                    .RegisterParameter<Function>("fn")
                    .VariadicAfter(0)
                    .Returns<object>()
                    .Build()
            ));


            environment.Define("proc",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        Process cmd = new Process();
                        cmd.StartInfo.FileName = args.Get<string>(0);
                        cmd.StartInfo.Arguments = args.Get<string>(1);
                        cmd.StartInfo.RedirectStandardOutput = args.Get<bool>(2);
                        cmd.StartInfo.RedirectStandardInput = args.Get<bool>(3);
                        cmd.StartInfo.CreateNoWindow = true;
                        cmd.StartInfo.UseShellExecute = false;
                        cmd.StartInfo.WorkingDirectory = args.Get<string>(4);
                        cmd.Start();

                        string result = args.Get<bool>(2) ? cmd.StandardOutput.ReadToEnd() : "";
                        cmd.WaitForExit();

                        return QualifiedObjectBuilder.BuildString(result);

                    })
                    .RegisterParameter<string>("filename")
                    .RegisterParameter<string>("arguments", QualifiedObjectBuilder.BuildString(""))
                    .RegisterParameter<bool>("redirectStandardOutput", QualifiedObjectBuilder.BuildBoolean(true))
                    .RegisterParameter<bool>("redirectStandardInput", QualifiedObjectBuilder.BuildBoolean(true))
                    .RegisterParameter<string>("workingDirectory", QualifiedObjectBuilder.BuildString(""))
                    .Returns<string>()
                    .Build()),
                true);

            environment.Define("exit",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        throw new ExitException(args.Get<int>(0));
                    }
                    ).RegisterParameter<int>("code")
                    .ReturnsEmpty()
                    .Build())
                );


            #region PackageManager

            pmNamespace.Define("Package",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        bool force = args.Get<bool>(2);
                        string outDir = args.Get<string>(1);
                        if (!Directory.Exists(outDir))
                        {
                            Directory.CreateDirectory(outDir);
                        }
                        using (StreamReader r = new StreamReader(args.Get<string>(0)))
                        {
                            string json = r.ReadToEnd();
                            List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);

                            foreach (Package pkg in items)
                            {
                                if (items.FindAll(p => p.Version == pkg.Version && p.Name == pkg.Name).Count() > 1)
                                {
                                    throw new Exception($"Duplicate package ({pkg.Name}) and version ({pkg.Version}) in manifest");
                                }
                                if (!force)
                                {
                                    string potentialDir = Path.Combine(outDir, $"{pkg.Name}/{pkg.Version}");
                                    if (Directory.Exists(potentialDir))
                                    {
                                        throw new Exception($"Package ({pkg.Name}) with version ({pkg.Version}) already exists");
                                    }
                                }
                            }
                            foreach (Package pkg in items)
                            {
                                string potentialDir = Path.Combine(outDir, $"{pkg.Name}/{pkg.Version}");
                                if (!Directory.Exists(potentialDir))
                                {
                                    Directory.CreateDirectory(potentialDir);
                                }
                                foreach (string file in pkg.Files)
                                {
                                    File.Copy(file, Path.Combine(potentialDir, Path.GetFileName(file)), force);
                                }
                            }
                            return QualifiedObjectBuilder.BuildEmptyValue();
                        }
                    })
                    .RegisterParameter<string>("filepath")
                    .RegisterParameter<string>("outDir")
                    .RegisterParameter<bool>("force", QualifiedObjectBuilder.BuildBoolean(false))
                    .ReturnsEmpty()
                    .Build()
                ));

            pmNamespace.Define("Fetch",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string moduleName = args.Get<string>(0);
                      string version = args.Get<string>(1);
                      string manifestPath = args.Get<string>(2);

                      using (StreamReader r = new StreamReader(manifestPath))
                      {
                          string json = r.ReadToEnd();
                          List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);

                          Package pkg = items.Find(p => p.Name == moduleName && p.Version == version);
                          if (pkg == null)
                          {
                              throw new Exception($"Unable to find package {moduleName} version {version} in manifest");
                          }
                          else
                          {
                              foreach (RemoteFile file in pkg.RemoteFiles)
                              {
                                  WebClient client = new WebClient();
                                  client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{file.Local}");
                              }
                          }

                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("moduleName")
                  .RegisterParameter<string>("version")
                  .RegisterParameter<string>("manifestHome", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json")))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("Remove",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string moduleName = args.Get<string>(0);
                      string version = args.Get<string>(1);
                      string manifestPath = args.Get<string>(2);

                      using (StreamReader r = new StreamReader(manifestPath))
                      {
                          string json = r.ReadToEnd();
                          List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);

                          Package pkg = items.Find(p => p.Name == moduleName && p.Version == version);
                          if (pkg == null)
                          {
                              throw new Exception($"Unable to find package {moduleName} version {version} in manifest");
                          }
                          else
                          {
                              foreach (RemoteFile file in pkg.RemoteFiles)
                              {
                                  File.Delete($"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{file.Local}");
                              }
                          }

                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("moduleName")
                  .RegisterParameter<string>("version")
                  .RegisterParameter<string>("manifestHome", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json")))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("Fetch-All-Latest",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string manifestPath = args.Get<string>(0);
                      bool showStatus = args.Get<bool>(1);

                      using (StreamReader r = new StreamReader(manifestPath))
                      {
                          string json = r.ReadToEnd();
                          List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);
                          HashSet<string> completedPackages = new HashSet<string>();
                          foreach(Package pkg in items)
                          {
                              if (completedPackages.Contains(pkg.Name))
                              {
                                  continue;
                              }
                              List<Package> versions = items.FindAll(p => p.Name == pkg.Name);
                              versions.Sort((a, b) => a.Version.CompareTo(b.Version));
                              Package latest = versions.First();
                              if (showStatus)
                              {
                                  Console.WriteLine("==========================================");
                                  Console.WriteLine($"Found package: {latest.Name}@{latest.Version} in manifest");
                                  Console.ForegroundColor = ConsoleColor.DarkYellow;
                                  Console.WriteLine("Installing package...");
                                  Console.ResetColor();
                              }
                              var cursorLeft = Console.CursorLeft;
                              var cursorTop = Console.CursorTop;
                              int counter = 1;
                              foreach (RemoteFile file in latest.RemoteFiles)
                              {
                                  if (showStatus)
                                  {
                                      Console.SetCursorPosition(cursorLeft, cursorTop);
                                      Console.WriteLine($"Downloading files [{new string('#', counter).PadRight(latest.RemoteFiles.Count(), '.')}]");
                                  }
                                  
                                  WebClient client = new WebClient();
                                  client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{file.Local}");
                                  counter++;
                              }
                              if (showStatus)
                              {
                                  Console.SetCursorPosition(cursorLeft, cursorTop);
                                  Console.Write($"Downloading files ");
                                  Console.ForegroundColor = ConsoleColor.Green;
                                  Console.WriteLine($"[{new string('#', counter).PadRight(latest.RemoteFiles.Count(), '.')}]");
                                  Console.ResetColor();
                                  Console.WriteLine($"Finished installing {latest.Name}@{latest.Version}");
                              }
                              
                              completedPackages.Add(pkg.Name);
                          }

                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("manifestHome", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json")))
                  .RegisterParameter<bool>("showStatus", QualifiedObjectBuilder.BuildBoolean(true))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("Update",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string remoteInfoPath = args.Get<string>(0);


                      using (StreamReader r = new StreamReader(remoteInfoPath))
                      {
                          string json = r.ReadToEnd();
                          RemoteFile file = JsonConvert.DeserializeObject<RemoteFile>(json);

                          WebClient client = new WebClient();
                          client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/{file.Local}");
                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("remoteInfoPath", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "remote.json")))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("List",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       string manifestPath = args.Get<string>(0);

                       using (StreamReader r = new StreamReader(manifestPath))
                       {
                           string json = r.ReadToEnd();
                           List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);
                           HashSet<string> completedPackages = new HashSet<string>();
                           foreach (Package pkg in items)
                           {
                               if (completedPackages.Contains(pkg.Name))
                               {
                                   continue;
                               }
                               List<Package> versions = items.FindAll(p => p.Name == pkg.Name);
                               versions.Sort((a, b) => a.Version.CompareTo(b.Version));
                               foreach (Package v in versions)
                               {
                                   Console.WriteLine($"{v.Name}@{v.Version}");
                               }
                               completedPackages.Add(pkg.Name);
                           }

                           return QualifiedObjectBuilder.BuildEmptyValue();
                       }
                   })
                   .RegisterParameter<string>("manifestHome", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json")))
                   .ReturnsEmpty()
                   .Build()
               ));

            pmNamespace.Define("Install-From-Remote",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string moduleName = args.Get<string>(0);
                      string version = args.Get<string>(1);
                      string manifestPath = args.Get<string>(3);

                      using (StreamReader r = new StreamReader(manifestPath))
                      {
                          string json = r.ReadToEnd();
                          List<Package> items = JsonConvert.DeserializeObject<List<Package>>(json);

                          Package pkg = items.Find(p => p.Name == moduleName && p.Version == version);
                          if (pkg == null)
                          {
                              throw new Exception($"Unable to find package {moduleName} version {version} in manifest");
                          }
                          else
                          {
                              string pkgName = args.Get<string>(2) == string.Empty ? $"{pkg.Name}@{pkg.Version}" : args.Get<string>(2);
                              foreach (RemoteFile file in pkg.RemoteFiles)
                              {
                                  WebClient client = new WebClient();
                                  client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{pkgName}/{file.Local}");
                              }
                          }

                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("moduleName")
                  .RegisterParameter<string>("version")
                  .RegisterParameter<string>("nameOverride", QualifiedObjectBuilder.BuildString(string.Empty))
                  .RegisterParameter<string>("manifestHome", QualifiedObjectBuilder.BuildString(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location), "manifest.json")))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("Install-From-Local",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string manifestPath = args.Get<string>(0);

                      using (StreamReader r = new StreamReader(manifestPath))
                      {
                          string json = r.ReadToEnd();
                          Package pkg = JsonConvert.DeserializeObject<Package>(json);

                          if (pkg == null)
                          {
                              throw new Exception($"Package not found in manifest");
                          }
                          else
                          {
                              string pkgName = args.Get<string>(1) == string.Empty ? $"{pkg.Name}@{pkg.Version}" : args.Get<string>(1);
                              foreach (string file in pkg.Files)
                              {
                                  File.Copy(file, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{pkgName}/{Path.GetFileName(file)}");
                              }
                          }

                          return QualifiedObjectBuilder.BuildEmptyValue();
                      }
                  })
                  .RegisterParameter<string>("manifestPath")
                  .RegisterParameter<string>("nameOverride", QualifiedObjectBuilder.BuildString(string.Empty))
                  .ReturnsEmpty()
                  .Build()
              ));

            #endregion

            #region NativeList

            nativeListNamespace.Define("Create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNativeList(args.ToList().Select(x => x.Value).ToList());
                  })
                  .VariadicAfter(-1)
                  .VariadicType<object>()
                  .Returns<NativeList>()
                  .Build()
              ));

            nativeListNamespace.Define("CreateEmpty",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNativeList(new NativeList(args.Get<DataType>(0)));
                  })
                  .RegisterParameter<DataType>("type")
                  .Returns<NativeList>()
                  .Build()
              ));

            nativeListNamespace.Define("IsEmpty",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);

                      return QualifiedObjectBuilder.BuildBoolean(ls.IsEmpty());
                  })
                  .RegisterParameter<NativeList>("ls")
                  .Returns<bool>()
                  .Build()
              ));

            nativeListNamespace.Define("Add",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      ls.AddRange(args.ToList(0).Select(x => x.Value).ToList());
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<NativeList>("ls")
                  .VariadicAfter(0)
                  .VariadicType<object>()
                  .ReturnsEmpty()
                  .Build()
              ));

            nativeListNamespace.Define("RemoveAt",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      ls.Remove(args.Get<int>(1));
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<double>("index")
                  .ReturnsEmpty()
                  .Build()
              ));

            nativeListNamespace.Define("Remove",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      ls.Remove(args.Get(1));
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<object>("item")
                  .ReturnsEmpty()
                  .Build()
              ));

            nativeListNamespace.Define("Find",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      return ls.Find(args.Get<Function>(1));
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<Function>("predicate")
                  .Returns<object>()
                  .Build()
              ));

            nativeListNamespace.Define("ForEach",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      ls.ForEach(args.Get<Function>(1));
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<Function>("predicate")
                  .Returns<object>()
                  .Build()
              ));

            #endregion


            environment.Define("PackageManager", QualifiedObjectBuilder.BuildNamespace(pmNamespace));
            environment.Define("NativeList", QualifiedObjectBuilder.BuildNamespace(nativeListNamespace));

            return environment;
        }
    }
}
