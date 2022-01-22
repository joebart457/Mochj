using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleExport
{
    class Program
    {
        static void Main(string[] args)
        {

            //string[] keywords = new string[x];
            //var shellFile = ShellFile.FromFilePath(@"C:\Users\joeba\Pictures\Saved Pictures\download.jpg");
            var shellFile = ShellFile.FromFilePath(@"C:\Users\joeba\Desktop\bridgetest\keywrods.txt");
            //shellFile.Properties.System.Keywords.Value = keywords;
            var tags = (string[])shellFile.Properties.System.Keywords.ValueAsObject;
            tags = tags ?? new string[0];

            if (tags.Length != 0)
            {
                foreach (string str in tags)
                {
                    Console.WriteLine(str);
                }
            }
            Console.ReadLine();
        }
    }
}
