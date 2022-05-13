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
            Mochj._Storage.Environment consoleNamespace = new Mochj._Storage.Environment(null).WithAlias("Console");


            consoleNamespace.Define("WriteLine",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("WriteLine")
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
                  .Named("Write")
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
                  .Named("ReadLine")
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
                  .Named("ReadKey")
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
                  .Named("Read")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.Read().ToString());
                  })
                  .Returns<string>()
                  .Build()
              ));

            #region Get

            consoleNamespace.Define("CursorLeft",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("CursorLeft")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.CursorLeft);
                  })
                  .Returns<int>()
                  .Build()
              ));

            consoleNamespace.Define("CursorTop",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("CursorTop")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.CursorTop);
                  })
                  .Returns<int>()
                  .Build()
              ));

            consoleNamespace.Define("WindowTop",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("WindowTop")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.WindowTop);
                  })
                  .Returns<int>()
                  .Build()
              ));

            consoleNamespace.Define("WindowHeight",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("WindowHeight")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.WindowHeight);
                  })
                  .Returns<int>()
                  .Build()
              ));

            consoleNamespace.Define("WindowLeft",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("WindowLeft")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.WindowLeft);
                  })
                  .Returns<int>()
                  .Build()
              ));

            consoleNamespace.Define("WindowWidth",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("WindowWidth")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildNumber(Console.WindowWidth);
                  })
                  .Returns<int>()
                  .Build()
              ));

            #endregion

            #region Set

            consoleNamespace.Define("SetCursorLeft",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetCursorLeft")
                  .Action((Args args) =>
                  {
                      Console.CursorLeft = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetCursorTop",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetCursorTop")
                  .Action((Args args) =>
                  {
                      Console.CursorTop = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetWindowTop",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetWindowTop")
                  .Action((Args args) =>
                  {
                      Console.WindowTop = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetWindowHeight",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetWindowHeight")
                  .Action((Args args) =>
                  {
                      Console.WindowHeight = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetWindowLeft",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetWindowLeft")
                  .Action((Args args) =>
                  {
                      Console.WindowLeft = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetWindowWidth",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetWindowWidth")
                  .Action((Args args) =>
                  {
                      Console.WindowWidth = args.Get<int>(0);
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("value")
                  .ReturnsEmpty()
                  .Build()
              ));

            #endregion

            consoleNamespace.Define("Clear",
              QualifiedObjectBuilder.BuildFunction( 
                  new NativeFunction()
                  .Named("Clear")
                  .Action((Args args) =>
                  {
                      Console.Clear();
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetCursorPosition",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetCursorPosition")
                  .Action((Args args) =>
                  {
                      Console.SetCursorPosition(args.Get<int>(0), args.Get<int>(1));
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<int>("left", QualifiedObjectBuilder.BuildNumber(Console.CursorLeft))
                  .RegisterParameter<int>("top", QualifiedObjectBuilder.BuildNumber(Console.CursorTop))
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("ForegroundColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ForegroundColor")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.ForegroundColor.ToString());
                  })
                  .Returns<string>()
                  .Build()
              ));

            consoleNamespace.Define("BackgroundColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("BackgroundColor")
                  .Action((Args args) =>
                  {
                      return QualifiedObjectBuilder.BuildString(Console.BackgroundColor.ToString());
                  })
                  .Returns<string>()
                  .Build()
              ));

            consoleNamespace.Define("SetForegroundColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetForegroundColor")
                  .Action((Args args) =>
                  {
                      string fg = args.Get<string>(0);
                      if (Enum.TryParse<ConsoleColor>(fg, out var fgColor))
                      {
                          Console.ForegroundColor = fgColor;
                          return QualifiedObjectBuilder.BuildEmptyValue();

                      }
                      else
                      {
                          throw new Exception($"unable to parse string(s) to ConsoleColor: '{fg}'");
                      }
                  })
                  .RegisterParameter<string>("foregroundColor")
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("SetBackgroundColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetBackgroundColor")
                  .Action((Args args) =>
                  {
                      string bg = args.Get<string>(0);
                      if (Enum.TryParse<ConsoleColor>(bg, out var bgColor))
                      {
                          Console.BackgroundColor = bgColor;
                          return QualifiedObjectBuilder.BuildEmptyValue();

                      }
                      else
                      {
                          throw new Exception($"unable to parse string(s) to ConsoleColor: '{bg}'");
                      }
                  })
                  .RegisterParameter<string>("backgroundColor")
                  .ReturnsEmpty()
                  .Build()
              ));


            consoleNamespace.Define("SetColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("SetColor")
                  .Action((Args args) =>
                  {
                      string fg = args.Get<string>(0);
                      string bg = args.Get<string>(1);

                      if (Enum.TryParse<ConsoleColor>(fg, out var fgColor)
                      && Enum.TryParse<ConsoleColor>(bg, out var bgColor))
                      {
                          Console.ForegroundColor = fgColor;
                          Console.BackgroundColor = bgColor;
                      } else
                      {
                          throw new Exception($"unable to parse string(s) to ConsoleColor: '{fg}' and '{bg}'");
                      }
                      
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("fg", QualifiedObjectBuilder.BuildString(Console.ForegroundColor.ToString()))
                  .RegisterParameter<string>("bg", QualifiedObjectBuilder.BuildString(Console.BackgroundColor.ToString()))
                  .ReturnsEmpty()
                  .Build()
              ));

            consoleNamespace.Define("ResetColor",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("ResetColor")
                  .Action((Args args) =>
                  {
                      Console.ResetColor();
                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .ReturnsEmpty()
                  .Build()
              ));

            environment.Define("Console", QualifiedObjectBuilder.BuildNamespace(consoleNamespace));

            return 0;
        }
    }
}

