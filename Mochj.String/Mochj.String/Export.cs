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
            Mochj._Storage.Environment stringNamespace = new Mochj._Storage.Environment(null).WithAlias("String");


            stringNamespace.Define("FromChar",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("FromChar")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int width = args.Get<int>(1);
                      return QualifiedObjectBuilder.BuildString(new string(source.FirstOrDefault(), width));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("width")
                  .Returns<string>()
                  .Build()
              ));


            stringNamespace.Define("Replace",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Replace")
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

            stringNamespace.Define("Contains",
               QualifiedObjectBuilder.BuildFunction(
                   new NativeFunction()
                   .Named("Contains")
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

            stringNamespace.Define("Length",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Length")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      return QualifiedObjectBuilder.BuildNumber(source.Length);
                  })
                  .RegisterParameter<string>("source")
                  .Returns<int>()
                  .Build()
              ));

            stringNamespace.Define("Substring",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Substring")
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

            stringNamespace.Define("Remove",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Remove")
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

            stringNamespace.Define("At",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("At")
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

            stringNamespace.Define("Trim",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Trim")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.Trim());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("ToLower",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ToLower")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.ToLower());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("ToUpper",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ToUpper")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);

                      return QualifiedObjectBuilder.BuildString(source.ToUpper());
                  })
                  .RegisterParameter<string>("source")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("Split",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Split")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      string splitOn = args.Get<string>(1);

                      List<QualifiedObject> results = new List<QualifiedObject>();

                      results.AddRange(source.Split(new[] { splitOn }, StringSplitOptions.None).Select(x => QualifiedObjectBuilder.BuildString(x)));
                      return QualifiedObjectBuilder.BuildNativeList(results);
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("splitOn")
                  .Returns<NativeList>()
                  .Build()
              ));

            stringNamespace.Define("StartsWith",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("StartsWith")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      string testFor = args.Get<string>(1);

                      return QualifiedObjectBuilder.BuildBoolean(source.StartsWith(testFor));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<string>("testFor")
                  .Returns<bool>()
                  .Build()
              ));

            stringNamespace.Define("PadLeft",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("PadLeft")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int totalWidth = args.Get<int>(1);
                      string padChar = args.Get<string>(2);

                      return QualifiedObjectBuilder.BuildString(source.PadLeft(totalWidth, padChar.FirstOrDefault()));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("totalWidth")
                  .RegisterParameter<string>("padChar")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("PadRight",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("PadRight")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int totalWidth = args.Get<int>(1);
                      string padChar = args.Get<string>(2);

                      return QualifiedObjectBuilder.BuildString(source.PadRight(totalWidth, padChar.FirstOrDefault()));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("totalWidth")
                  .RegisterParameter<string>("padChar")
                  .Returns<string>()
                  .Build()
              ));

            stringNamespace.Define("IsNullOrWhiteSpace",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("IsNullOrWhiteSpace")
                    .Action((Args args) =>
                    {
                        string source = args.Get<string>(0);

                        return QualifiedObjectBuilder.BuildBoolean(string.IsNullOrWhiteSpace(source));
                    })
                    .RegisterParameter<string>("source")
                    .Returns<bool>()
                    .Build()
                ));

            stringNamespace.Define("IsNullOrEmpty",
                QualifiedObjectBuilder.BuildFunction(
                    new NativeFunction()
                    .Named("IsNullOrEmpty")
                    .Action((Args args) =>
                    {
                        string source = args.Get<string>(0);

                        return QualifiedObjectBuilder.BuildBoolean(string.IsNullOrEmpty(source));
                    })
                    .RegisterParameter<string>("source")
                    .Returns<bool>()
                    .Build()
                ));

            stringNamespace.Define("Repeat",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Repeat")
                  .Action((Args args) =>
                  {
                      string source = args.Get<string>(0);
                      int count = args.Get<int>(1);

                      return QualifiedObjectBuilder.BuildString(string.Concat(Enumerable.Repeat(source, count)));
                  })
                  .RegisterParameter<string>("source")
                  .RegisterParameter<int>("count")
                  .Returns<string>()
                  .Build()
              ));

            environment.Define("String", QualifiedObjectBuilder.BuildNamespace(stringNamespace));

            return 0;
        }
    }
}
