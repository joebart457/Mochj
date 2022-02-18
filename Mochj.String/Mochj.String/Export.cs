using Mochj.Builders;
using Mochj.Models;
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
            Mochj._Storage.Environment stringNamespace = new Mochj._Storage.Environment(null);


            stringNamespace.Define("replace",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      string oldString = args.Get<string>(1);
                      string newString = args.Get<string>(2);
                      return QualifiedObjectBuilder.BuildString(source.Replace(oldString, newString));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("oldString")
                  .RegisterParameter<string>("newString")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("contains",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Action((Args args) =>
                   {
                       string source = args.Get<string>(0);
                       string search = args.Get<string>(1);
                       return QualifiedObjectBuilder.BuildBoolean(source.Contains(search));
                   })
                   .RegisterParameter<string>("source")
                   .RegisterParameter<string>("search")
                   .Returns<bool>()
                   .Build()
               ));

            stringNamespace.Define("length",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      return QualifiedObjectBuilder.BuildNumber(source.Length);
                  })
                  .RegisterParameter<string>("source")
                  .Returns<int>()
                  .Build()
              ));

            stringNamespace.Define("substring",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int startIndex = args.Get<int>(1);
                      int endIndex = args.Get<int>(2);
                      return QualifiedObjectBuilder.BuildString(source.Substring(startIndex, endIndex));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("startIndex")
                  .RegisterParameter<int>("endIndex")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("remove",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int startIndex = args.Get<int>(1);
                      int endIndex = args.Get<int>(2);
                      return QualifiedObjectBuilder.BuildString(source.Remove(startIndex, endIndex));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("startIndex")
                  .RegisterParameter<int>("endIndex")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("at",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int index = args.Get<int>(1);
                      return QualifiedObjectBuilder.BuildString(source.ElementAt(index).ToString());
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("index")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("trim",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.Trim());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("to-lower",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.ToLower());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("to-upper",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.ToUpper());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("split",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      string splitOn = args.Get<string>(1);

                      List<QualifiedObject> results = new List<QualifiedObject>();

                      results.AddRange(source.Split(new[] { splitOn }, StringSplitOptions.None).Select(x => QualifiedObjectBuilder.BuildString(x)));
                      return QualifiedObjectBuilder.BuildList(results);
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("splitOn")
                  .Returns<object>()
                  .Build()
              ));

            environment.Define("String", QualifiedObjectBuilder.BuildNamespace(stringNamespace));

            return 0;
        }
    }
}
