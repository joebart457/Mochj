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
            Mochj._Storage.Environment debugNamespace = new Mochj._Storage.Environment(null);


            debugNamespace.Define("show",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {

                      Console.WriteLine(args.Get<Mochj._Storage.Environment>(0).ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<Mochj._Storage.Environment>("env")
                  .ReturnsEmpty()
                  .Build()
              ));

            debugNamespace.Define("top",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {

                      Console.WriteLine(environment.Top().ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .ReturnsEmpty()
                  .Build()
              ));

            debugNamespace.Define("help",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {

                      Console.WriteLine(args.Get<Function>(0).ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<Function>("fn")
                  .ReturnsEmpty()
                  .Build()
              ));

            environment.Define("Debug", QualifiedObjectBuilder.BuildNamespace(debugNamespace));

            return 0;
        }
    }
}

