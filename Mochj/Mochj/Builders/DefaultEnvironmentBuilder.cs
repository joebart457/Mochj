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

            _Storage.Environment pmNamespace = new Mochj._Storage.Environment(null);

            environment.Define("version", QualifiedObjectBuilder.BuildString("1.01"));

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

            environment.Define("to-string",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) => { return QualifiedObjectBuilder.BuildString(Convert.ToString(args.Get<object>(0))); })
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


            environment.Define("Empty", QualifiedObjectBuilder.EmptyFunction());


            var lsNode =
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .RegisterParameter<BoundFn>("nextVal")
                    .RegisterParameter<object>("data")
                    .ReturnsEmpty()
                    .Build();

            environment.Define("ls-node", QualifiedObjectBuilder.BuildFunction(lsNode));

            environment.Define("list",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        return QualifiedObjectBuilder.BuildList(args.ToList().Select(a => a.Value));
                    })
                    .VariadicAfter(-1)
                    .Returns<BoundFn>()
                    .Build()
                ));

            environment.Define("ls-size",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (args.Get(0).Equals(QualifiedObjectBuilder.EmptyFunction())) return QualifiedObjectBuilder.BuildNumber(0);
                        BoundFn lsFn = args.Get<BoundFn>(0);
                        int count = 1;
                        while (!lsFn.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                            count++;
                        }

                        return QualifiedObjectBuilder.BuildNumber(count);
                    })
                    .RegisterParameter<BoundFn>("ls")
                    .Returns<int>()
                    .Build()
                ));

            environment.Define("ls-get",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (args.Get(0).Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            throw new Exception($"unable to retrieve value at index {args.Get<int>(1)}; list was empty");
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        int targetIndex = args.Get<int>(1);
                        int count = 0;
                        while (count != targetIndex)
                        {
                            if (lsFn.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                            {
                                throw new Exception($"unable to retrieve value at index {targetIndex}; index out of range");
                            }
                            lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                            count++;
                        }

                        return lsFn.BoundArguments.Get("data");
                    })
                    .RegisterParameter<BoundFn>("ls")
                    .RegisterParameter<int>("index")
                    .Returns<object>()
                    .Build()
                ));

            environment.Define("ls-add",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        List<Argument> argsToBind = new List<Argument>();
                        argsToBind.Add(new Argument { Position = 0, Value = QualifiedObjectBuilder.EmptyFunction() });
                        argsToBind.Add(new Argument { Position = 1, Value = args.Get(1) });

                        Models.QualifiedObject nodeToAdd = QualifiedObjectBuilder.BuildFunction(
                            new BoundFn
                            {
                                hFunc = lsNode,
                                BoundArguments = new Args(lsNode.Params.PatchArguments(argsToBind))
                            });

                        if (args.Get(0).Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            return nodeToAdd;
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        while (!lsFn.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                        }

                        lsFn.BoundArguments.Replace("nextVal", nodeToAdd);
                        return args.Get(0);
                    })
                    .RegisterParameter<BoundFn>("ls")
                    .RegisterParameter<object>("data")
                    .Returns<Function>()
                    .Build()
                ));

            environment.Define("ls-remove",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        int targetIndex = args.Get<int>(1);

                        if (args.Get(0).Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            throw new Exception($"unable to remove value at {targetIndex}; list is empty");
                        }

                        BoundFn currentNode = args.Get<BoundFn>(0);
                        BoundFn previousNode = null;
                        int count = 0;
                        while (count != targetIndex)
                        {
                            if (currentNode.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                            {
                                throw new Exception($"unable to remove value at index {targetIndex}; index out of range");
                            }
                            previousNode = currentNode;
                            currentNode = currentNode.BoundArguments.Get<BoundFn>("nextVal");
                            count++;
                        }
                        if (previousNode == null)
                        {
                            // Chop off head
                            return currentNode.BoundArguments.Get("nextVal");
                        }
                        previousNode.BoundArguments.Replace("nextVal", currentNode.BoundArguments.Get("nextVal"));
                        return args.Get(0);

                    })
                    .RegisterParameter<BoundFn>("ls")
                    .RegisterParameter<int>("index")
                    .Returns<Function>()
                    .Build()
                ));

            environment.Define("ls-tail",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Action((Args args) =>
                    {
                        if (args.Get(0).Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            return args.Get(0);
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        Models.QualifiedObject lsFnObject = args.Get(0);
                        while (!lsFn.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                        {
                            lsFnObject = lsFn.BoundArguments.Get("nextVal");
                            lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                        }

                        return lsFnObject;
                    })
                    .RegisterParameter<BoundFn>("ls")
                    .Returns<Function>()
                    .Build()
                ));

            environment.Define("ls-foreach",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       if (args.Get(0).Equals(QualifiedObjectBuilder.BuildEmptyValue())) return QualifiedObjectBuilder.EmptyFunction();
                       BoundFn lsFn = args.Get<BoundFn>(0);
                       Function fn = args.Get<Function>(1);
                       int count = 0;
                       while (true)
                       {
                           IList<Argument> arguments = new List<Argument>();
                           arguments.Add(new Argument { Alias = "data", Position = 0, Value = lsFn.BoundArguments.Get("data") });
                           arguments.Add(new Argument { Alias = "index", Position = 1, Value = QualifiedObjectBuilder.BuildNumber(count) });

                           fn.Call(fn.ResolveArguments(arguments));

                           if (lsFn.BoundArguments.Get("nextVal").Equals(QualifiedObjectBuilder.EmptyFunction()))
                           {
                               break;
                           }

                           lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                           count++;
                       }

                       return QualifiedObjectBuilder.BuildEmptyValue();
                   })
                   .RegisterParameter<BoundFn>("ls")
                   .RegisterParameter<Function>("fn")
                   .ReturnsEmpty()
                   .Build()
               ));



            // control flow

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


            // Comparisons

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


            // Package

            pmNamespace.Define("package",
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

            pmNamespace.Define("fetch",
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

            pmNamespace.Define("fetch-all-latest",
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
                          foreach(Package pkg in items)
                          {
                              if (completedPackages.Contains(pkg.Name))
                              {
                                  continue;
                              }
                              List<Package> versions = items.FindAll(p => p.Name == pkg.Name);
                              versions.Sort((a, b) => a.Version.CompareTo(b.Version));
                              Package latest = versions.First();
                              foreach (RemoteFile file in latest.RemoteFiles)
                              {
                                  WebClient client = new WebClient();
                                  client.DownloadFile(file.RemoteUrl, $"{Path.GetDirectoryName(System.Reflection.Assembly.GetCallingAssembly().Location)}/pkg/{file.Local}");
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

            pmNamespace.Define("update",
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


            environment.Define("PackageManager", QualifiedObjectBuilder.BuildNamespace(pmNamespace));

            return environment;
        }
    }
}
