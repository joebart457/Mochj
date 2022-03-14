using Mochj.Builders;
using Mochj.Models.Fn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportItems
{
    public class Export
    {
        public int Setup(Mochj._Storage.Environment environment)
        {
            Mochj._Storage.Environment consoleNamespace = new Mochj._Storage.Environment(null);


            consoleNamespace.Define("WriteLine",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      Console.WriteLine(args.Get(0).Object);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<object>("obj")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("Write",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      Console.Write(args.Get(0).Object);

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<object>("obj")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("ReadLine",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.ReadLine());
                  })
                  .Returns<string>()
                  .Build()
              ));

            consoleNamespace.Define("ReadKey",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.ReadKey().ToString());
                  })
                  .Returns<string>()
                  .Build()
              ));

            consoleNamespace.Define("Read",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.Read().ToString());
                  })
                  .Returns<string>()
                  .Build()
              ));

            environment.Define("Console", QualifiedObjectBuilder.BuildNamespace(consoleNamespace));

            return 0;
        }
    }
}

