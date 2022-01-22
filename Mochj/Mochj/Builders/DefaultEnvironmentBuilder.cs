using Mochj.Models.ControlFlow;
using Mochj.Models.Fn;
using Mochj.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mochj.Builders
{
    static class DefaultEnvironmentBuilder
    {
        private static _Storage.Environment _default = null;
        public static _Storage.Environment Default { get { return _default == null ? Build() : _default; } }
        public static _Storage.Environment Build()
        {
            if (_default != null) return _default;
            _Storage.Environment environment = new _Storage.Environment(null);
            _default = environment;

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
                    .Action((Args args) => {
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
                    .Action((Args args) => {
                        StringBuilder result = new StringBuilder(args.Get<string>(0));
                        
                        var remainingArgs = args.ToList(0);
                        foreach(Argument argument in remainingArgs)
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
                    .Action((Args args) => {
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
                    .Action((Args args) => {
                        BoundFn fn = args.Get<BoundFn>(0);
                        return fn.BoundArguments.Get(args.Get<string>(1));
                    })
                    .RegisterParameter<BoundFn>("fn")
                    .RegisterParameter<string>("key")
                    .Returns<object>()
                    .Build()
                ));

            var empty = new NativeFunction()
                    .Action((Args args) => {
                        return QualifiedObjectBuilder.BuildEmptyValue();
                    })
                    .ReturnsEmpty()
                    .Build();

            var emptyObject = QualifiedObjectBuilder.BuildFunction(empty);

            environment.Define("Empty", QualifiedObjectBuilder.BuildFunction(empty));


            var lsNode = 
                    new NativeFunction()
                    .Action((Args args) => {
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
                    .Action((Args args) => {
                        IList<Argument> arguments = args.ToList();
                        Models.QualifiedObject previousNode = emptyObject;
                        foreach (Argument arg in arguments.Reverse())
                        {
                            List<Argument> argsToBind = new List<Argument>();
                            Models.QualifiedObject nextVal = previousNode;
                            argsToBind.Add(new Argument { Position = 0, Value = nextVal });
                            argsToBind.Add(new Argument { Position = 1, Value = arg.Value });

                            BoundFn currentNode = new BoundFn 
                            {
                                hFunc = lsNode,
                                BoundArguments = new Args(lsNode.Params.PatchArguments(argsToBind))
                            };
                            previousNode = QualifiedObjectBuilder.BuildFunction(currentNode);
                        }

                        return previousNode;
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
                        if (args.Get(0) == emptyObject) return QualifiedObjectBuilder.BuildNumber(0);
                        BoundFn lsFn = args.Get<BoundFn>(0);
                        int count = 1;
                        while (lsFn.BoundArguments.Get("nextVal") != emptyObject)
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
                        if (args.Get(0) == emptyObject)
                        {
                            throw new Exception($"unable to retrieve value at index {args.Get<int>(1)}; list was empty");
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        int targetIndex = args.Get<int>(1);
                        int count = 0;
                        while (count != targetIndex)
                        {
                            if (lsFn.BoundArguments.Get("nextVal") == emptyObject)
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
                        argsToBind.Add(new Argument { Position = 0, Value = emptyObject });
                        argsToBind.Add(new Argument { Position = 1, Value = args.Get(2) });

                        Models.QualifiedObject nodeToAdd = QualifiedObjectBuilder.BuildFunction(
                            new BoundFn
                            {
                                hFunc = lsNode,
                                BoundArguments = new Args(lsNode.Params.PatchArguments(argsToBind))
                            });

                        if (args.Get(0) == emptyObject)
                        {
                            return nodeToAdd;
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        while (lsFn.BoundArguments.Get("nextVal") != emptyObject)
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

                        if (args.Get(0) == emptyObject)
                        {
                            throw new Exception($"unable to remove value at {targetIndex}; list is empty");
                        }

                        BoundFn currentNode = args.Get<BoundFn>(0);
                        BoundFn previousNode = null;
                        int count = 0;
                        while (count != targetIndex)
                        {
                            if (currentNode.BoundArguments.Get("nextVal") == emptyObject)
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
                        if (args.Get(0) == emptyObject)
                        {
                            return args.Get(0);
                        }

                        BoundFn lsFn = args.Get<BoundFn>(0);
                        Models.QualifiedObject lsFnObject = args.Get(0);
                        while (lsFn.BoundArguments.Get("nextVal") != emptyObject)
                        {
                            lsFn = lsFn.BoundArguments.Get<BoundFn>("nextVal");
                            lsFnObject = lsFn.BoundArguments.Get("nextVal");
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
                       if (args.Get(0) == emptyObject) return QualifiedObjectBuilder.BuildEmptyValue();
                       BoundFn lsFn = args.Get<BoundFn>(0);
                       Function fn = args.Get<Function>(1);
                       int count = 0;
                       while (true)
                       {
                           IList<Argument> arguments = new List<Argument>();
                           arguments.Add(new Argument { Alias = "data", Position = 0, Value = lsFn.BoundArguments.Get("data") });
                           arguments.Add(new Argument { Alias = "index", Position = 1, Value = QualifiedObjectBuilder.BuildNumber(count) });

                           fn.Call(fn.ResolveArguments(arguments));

                           if (lsFn.BoundArguments.Get("nextVal") == emptyObject)
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
                       } else
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
                       bool condition = args.Get<bool>(0);
                       Function doThis = args.Get<Function>(1);

                       while (condition)
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
                   .RegisterParameter<bool>("condition")
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

            return environment;
        }
    }
}
