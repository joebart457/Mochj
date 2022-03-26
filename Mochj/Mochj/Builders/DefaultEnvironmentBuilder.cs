using Mochj._PackageManager.Models.Constants;
using Mochj.Models;
using Mochj.Models.ControlFlow;
using Mochj.Models.Fn;
using Mochj.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        public static _Storage.Environment Build(bool _force = false)
        {
            if (!_force && _default != null) return _default;
            _Storage.Environment environment = new _Storage.Environment(null);
            _default = environment;

            _Storage.Environment convertNamespace = new _Storage.Environment(null).WithAlias("Convert");
            _Storage.Environment fnNamespace = new _Storage.Environment(null).WithAlias("Fn");
            _Storage.Environment pmNamespace = new _Storage.Environment(null).WithAlias("PackageManager");
            _Storage.Environment processNamespace = new _Storage.Environment(null).WithAlias("Process");
            _Storage.Environment nativeListNamespace = new _Storage.Environment(null).WithAlias("NativeList");

            environment.Define("version", QualifiedObjectBuilder.BuildString("2.20"));

            environment.Define("typeof",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("typeof")
                    .Action((Args args) =>
                    {
                        QualifiedObject obj = args.Get(0);

                        return QualifiedObjectBuilder.BuildTypeInfo(obj.Type);
                    })
                    .RegisterParameter<object>("any")
                    .Returns<DataType>()
                    .Build()
                ));

            environment.Define("nameof",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("nameof")
                    .Action((Args args) =>
                    {
                        QualifiedObject obj = args.Get(0);
                        if (obj.Type.Is(Enums.DataTypeEnum.Fn))
                        {
                            return QualifiedObjectBuilder.BuildString(TypeMediatorService.ToNativeType<Function>(obj).Name);
                        }

                        return QualifiedObjectBuilder.BuildString(string.Empty);
                    })
                    .RegisterParameter<object>("any")
                    .Returns<string>()
                    .Build()
                ));

            environment.Define("symbol",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("symbol")
                    .Action((Args args) =>
                    {
                        QualifiedObject obj = args.Get(0);
                        if (obj.Type.Is(Enums.DataTypeEnum.Fn))
                        {
                            var fn = TypeMediatorService.ToNativeType<Function>(obj);
                            return QualifiedObjectBuilder.BuildString((fn.Symbol == null? "" : fn.Symbol.ToString()));
                        }

                        return QualifiedObjectBuilder.BuildString(string.Empty);
                    })
                    .RegisterParameter<object>("any")
                    .Returns<string>()
                    .Build()
                ));

            environment.Define("println",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("println")
                    .Action((Args args) => { Console.WriteLine(args.Get<object>(0)); return QualifiedObjectBuilder.BuildEmptyValue(); })
                    .RegisterParameter<object>("a")
                    .ReturnsEmpty()
                    .Build()
                ));

            environment.Define("_unsafe_assign",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("_unsafe_assign")
                    .Action((Args args) => 
                    {
                        QualifiedObject assignable = args.Get(0);
                        QualifiedObject value = args.Get(1);
                        assignable.Object = value.Object;
                        assignable.Type = value.Type;
                        return QualifiedObjectBuilder.BuildEmptyValue(); 
                    })
                    .RegisterParameter<object>("assignable")
                    .RegisterParameter<object>("value")
                    .ReturnsEmpty()
                    .Build()
                ));

            #region Convert

            convertNamespace.Define("ToString",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("ToString")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildString($"{args.Get(0).Object}"); })
                    .RegisterParameter<object>("obj")
                    .Returns<string>()
                    .Build()
                ));

            #endregion

            #region TypeInitializations

            environment.Define("Empty", QualifiedObjectBuilder.EmptyFunction());

            environment.Define("Number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Number")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0)); })
                    .RegisterParameter<object>("convertibleObject")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("String",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("String")
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
                    .Named("Boolean")
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
                    .Named("add-number")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("addn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("addn")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) + args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("sub-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("sub-number")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) - args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("subn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("subn")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) - args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));

            environment.Define("mul-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("mul-number")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) * args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("muln",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("muln")
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) * args.Get<double>(1)); })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));


            environment.Define("div-number",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("div-number")
                    .Action((Args args) => {
                        double denominator = args.Get<double>(1);
                        if (denominator == 0) throw new Exception("Cannot divide by zero");
                        return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) / denominator); 
                    })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));
            environment.Define("divn",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("divn")
                    .Action((Args args) => {
                        double denominator = args.Get<double>(1);
                        if (denominator == 0) throw new Exception("Cannot divide by zero");
                        return QualifiedObjectBuilder.BuildNumber(args.Get<double>(0) / denominator);
                    })
                    .RegisterParameter<double>("a")
                    .RegisterParameter<double>("b")
                    .Returns<double>()
                    .Build()
                ));


            environment.Define("add-string",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("add-string")
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
                    .Named("adds")
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

            #region Fn

            fnNamespace.Define("Return",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Return")
                    .Action((Args args) => { 
                        if (args.Get(1) == QualifiedObjectBuilder.EmptyFunction())
                        {
                            throw new ReturnException(args.Get(0), null);
                        }
                        throw new ReturnException(args.Get(0), args.Get<Function>(1)); 
                    })
                    .RegisterParameter<object>("obj", QualifiedObjectBuilder.BuildEmptyValue())
                    .RegisterParameter<Function>("toFn", QualifiedObjectBuilder.EmptyFunction())
                    .Returns<object>()
                    .Build()
                ));

            fnNamespace.Define("Bind",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Bind")
                    .Action((Args args) =>
                    {
                        Function fn = args.Get<Function>(0);
                        Function boundFn = new BoundFn
                        {
                            Name = fn.Name,
                            Symbol = fn.Symbol,
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

            fnNamespace.Define("GetArgument",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("GetArgument")
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

            fnNamespace.Define("ReplaceArgument",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("ReplaceArgument")
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

            fnNamespace.Define("Success",
                QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("Success")
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

            fnNamespace.Define("Call",
                 QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Call")
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

            #endregion

            #region ControlFlow

            environment.Define("if",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("if")
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
                   .RegisterParameter<Function>("elseDo", QualifiedObjectBuilder.EmptyFunction())
                   .Returns<object>()
                   .Build()
               ));

            environment.Define("while",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("while")
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
                   .Named("break")
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
                   .Named("continue")
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
                    .Named("try")
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

            environment.Define("throw",
                 QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("throw")
                    .Action((Args args) =>
                    {
                        throw new Exception(args.Get<string>(0));
                    })
                    .RegisterParameter<string>("message")
                    .ReturnsEmpty()
                    .Build()
            ));

            #endregion

            #region Comparators

            environment.Define("equal",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("equal")
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
                   .Named("equals")
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
                   .Named("not-equal")
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
                   .Named("not-equals")
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
                   .Named("gt")
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
                   .Named("gte")
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
                   .Named("lt")
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
                   .Named("lte")
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
                   .Named("greater-than")
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
                   .Named("greater-than-equal")
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
                   .Named("less-than")
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
                   .Named("less-than-equal")
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
                   .Named("not")
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
                   .Named("and")
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
                   .Named("or")
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

            #region Process

            processNamespace.Define("Create",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Create")
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

            processNamespace.Define("Exit",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Exit")
                    .Action((Args args) =>
                    {
                        throw new ExitException(args.Get<int>(0));
                    }
                    ).RegisterParameter<int>("code")
                    .ReturnsEmpty()
                    .Build())
                );

            #endregion

            #region PackageManager

            pmNamespace.Define("ShowOutput",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("ShowOutput")
                    .Action((Args args) =>
                    {
                        bool showFlag = args.Get<bool>(0);

                        _PackageManager.PackageManager.ShowOutput(showFlag);
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<bool>("showFlag", QualifiedObjectBuilder.BuildBoolean(true))
                    .ReturnsEmpty()
                    .Build()
                ));

            pmNamespace.Define("Package",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Package")
                    .Action((Args args) =>
                    {
                        string settingsPath = args.Get<string>(0);
                        bool publishLocal = args.Get<bool>(1);

                        _PackageManager.PackageManager.Package(settingsPath, publishLocal);
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<string>("settingsPath")
                    .RegisterParameter<bool>("publishLocal", QualifiedObjectBuilder.BuildBoolean(false))
                    .ReturnsEmpty()
                    .Build()
                ));

            pmNamespace.Define("Validate",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Validate")
                    .Action((Args args) =>
                    {
                        string manifestPath = args.Get<string>(0);
                        _PackageManager.PackageManager.Validate(manifestPath);
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                    .ReturnsEmpty()
                    .Build()
                ));

            pmNamespace.Define("Use",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Use")
                    .Action((Args args) =>
                    {
                        string moduleName = args.Get<string>(0);
                        string version = args.Get<string>(1);
                        string manifestPath = args.Get<string>(2);
                        _Storage.Environment env = args.Get<_Storage.Environment>(3);

                        _PackageManager.PackageManager.Use(moduleName, version, manifestPath, env);

                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<string>("moduleName")
                    .RegisterParameter<string>("version")
                    .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                    .RegisterParameter<_Storage.Environment>("environment", QualifiedObjectBuilder.BuildNamespace(environment))
                    .ReturnsEmpty()
                    .Build()
                ));


            pmNamespace.Define("Fetch",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("Fetch")
                    .Action((Args args) =>
                    {
                        string moduleName = args.Get<string>(0);
                        string version = args.Get<string>(1);
                        string manifestPath = args.Get<string>(2);
                        bool force = args.Get<bool>(3);

                        _PackageManager.PackageManager.Fetch(moduleName, version, manifestPath, force);

                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<string>("moduleName")
                    .RegisterParameter<string>("version")
                    .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                    .RegisterParameter<bool>("force", QualifiedObjectBuilder.BuildBoolean(false))
                    .ReturnsEmpty()
                    .Build()
                ));

            pmNamespace.Define("FetchAllLatest",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("FetchAllLatest")
                   .Action((Args args) =>
                   {
                       string manifestPath = args.Get<string>(0);
                       bool force = args.Get<bool>(1);

                       _PackageManager.PackageManager.FetchAllLatest(manifestPath, force);

                       return QualifiedObjectBuilder.BuildEmptyValue();
                   })
                   .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                   .RegisterParameter<bool>("force", QualifiedObjectBuilder.BuildBoolean(false))
                   .ReturnsEmpty()
                   .Build()
               ));

            pmNamespace.Define("Uninstall",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Uninstall")
                  .Action((Args args) =>
                  {
                      string moduleName = args.Get<string>(0);
                      string version = args.Get<string>(1);
                      string manifestPath = args.Get<string>(2);
                      bool keepCached = args.Get<bool>(3);

                      _PackageManager.PackageManager.Uninstall(moduleName, version, manifestPath, keepCached);

                      return QualifiedObjectBuilder.BuildEmptyValue();

                  })
                  .RegisterParameter<string>("moduleName")
                  .RegisterParameter<string>("version")
                  .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                  .RegisterParameter<bool>("keepCached", QualifiedObjectBuilder.BuildBoolean(false))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("UninstallAll",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("UninstallAll")
                  .Action((Args args) =>
                  {
                      string manifestPath = args.Get<string>(0);

                      _PackageManager.PackageManager.UninstallAll(manifestPath);

                      return QualifiedObjectBuilder.BuildEmptyValue();

                  })
                  .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("Update",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Update")
                  .Action((Args args) =>
                  {
                      string remoteInfoPath = args.Get<string>(0);

                      _PackageManager.PackageManager.Update(remoteInfoPath);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("remoteInfoPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.RemoteInfoPath))
                  .ReturnsEmpty()
                  .Build()
              ));

            pmNamespace.Define("List",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("List")
                   .Action((Args args) =>
                   {
                       string manifestPath = args.Get<string>(0);

                       _PackageManager.PackageManager.GetAllPackageNamesAndVersions(manifestPath)
                       .ForEach(str => Console.WriteLine(str));

                       return QualifiedObjectBuilder.BuildEmptyValue();
                   })
                   .RegisterParameter<string>("manifestPath", QualifiedObjectBuilder.BuildString(DefaultPathConstants.ManifestPath))
                   .ReturnsEmpty()
                   .Build()
               ));


            #endregion

            #region NativeList

            nativeListNamespace.Define("Create",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Create")
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
                  .Named("CreateEmpty")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNativeList(new NativeList(args.Get<DataType>(0)));
                  })
                  .RegisterParameter<DataType>("type")
                  .Returns<NativeList>()
                  .Build()
              ));

            nativeListNamespace.Define("At",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("At")
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);

                      return ls.At(args.Get<int>(1));
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<int>("index")
                  .Returns<object>()
                  .Build()
              ));

            nativeListNamespace.Define("IsEmpty",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("IsEmpty")
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
                  .Named("Add")
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
                  .Named("RemoveAt")
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
                  .Named("Remove")
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
                  .Named("Find")
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
                  .Named("ForEach")
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

            nativeListNamespace.Define("ForEachWithIndex",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ForEach")
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      ls.ForEachWithIndex(args.Get<Function>(1));
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<Function>("predicate")
                  .Returns<object>()
                  .Build()
              ));

            nativeListNamespace.Define("Count",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Count")
                  .Action((Args args) =>
                  {
                      var ls = args.Get<NativeList>(0);
                      if (args.Get(1).Equals(QualifiedObjectBuilder.EmptyFunction()))
                      {
                          return QualifiedObjectBuilder.BuildNumber(ls.Count(null));
                      }
                      return QualifiedObjectBuilder.BuildNumber(ls.Count(args.Get<Function>(1)));
                  })
                  .RegisterParameter<NativeList>("ls")
                  .RegisterParameter<Function>("predicate", QualifiedObjectBuilder.EmptyFunction())
                  .Returns<int>()
                  .Build()
              ));

            #endregion


            environment.Define("Convert", QualifiedObjectBuilder.BuildNamespace(convertNamespace));
            environment.Define("Fn", QualifiedObjectBuilder.BuildNamespace(fnNamespace));
            environment.Define("PackageManager", QualifiedObjectBuilder.BuildNamespace(pmNamespace));
            environment.Define("Process", QualifiedObjectBuilder.BuildNamespace(processNamespace));
            environment.Define("NativeList", QualifiedObjectBuilder.BuildNamespace(nativeListNamespace));

            return environment;
        }
    }
}
