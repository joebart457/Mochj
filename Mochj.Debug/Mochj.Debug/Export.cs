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
            Mochj._Storage.Environment debugNamespace = new Mochj._Storage.Environment(null).WithAlias("Debug");


            debugNamespace.Define("Show",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Show")
                  .Action((Args args) =>
                  {

                      Console.WriteLine(args.Get<Mochj._Storage.Environment>(0).ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<Mochj._Storage.Environment>("env", QualifiedObjectBuilder.BuildNamespace(environment))
                  .ReturnsEmpty()
                  .Build()
              ));

            debugNamespace.Define("Top",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Top")
                  .Action((Args args) =>
                  {

                      Console.WriteLine(environment.Top().ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .ReturnsEmpty()
                  .Build()
              ));

            debugNamespace.Define("Help",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Help")
                  .Action((Args args) =>
                  {

                      Console.WriteLine(args.Get<Function>(0).ToString());

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<Function>("fn")
                  .ReturnsEmpty()
                  .Build()
              ));

            debugNamespace.Define("Exists",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Exists")
                  .Action((Args args) =>
                  {
                      string variable = args.Get<string>(0);
                      return QualifiedObjectBuilder.BuildBoolean(environment.Exists(variable));
                  })
                  .RegisterParameter<string>("var")
                  .RegisterParameter<Mochj._Storage.Environment>("env", QualifiedObjectBuilder.BuildNamespace(environment))
                  .Returns<bool>()
                  .Build()
              ));


            environment.Define("Debug", QualifiedObjectBuilder.BuildNamespace(debugNamespace));

            return 0;
        }
    }
}

