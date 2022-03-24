using Mochj.Builders;
using Mochj.Models.Fn;
using Mochj.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExportItems
{
    public class Export
    {
        public int Setup(Mochj._Storage.Environment environment)
        {
            Mochj._Storage.Environment webNamespace = new Mochj._Storage.Environment(null).WithAlias("Web");

            webNamespace.Define("Download",
              QualifiedObjectBuilder.BuildFunction(
                  new NativeFunction()
                  .Named("Download")
                  .Action((Args args) =>
                  {

                      WebClient client = new WebClient();
                      client.DownloadFile(args.Get<string>(0), args.Get<string>(1));

                      return QualifiedObjectBuilder.BuildEmptyValue();
                  })
                  .RegisterParameter<string>("url")
                  .RegisterParameter<string>("localDest")
                  .ReturnsEmpty()
                  .Build()
              ));


            environment.Define("Web", QualifiedObjectBuilder.BuildNamespace(webNamespace));
            
            return 0;
        }

    }
}

