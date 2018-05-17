using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GrubHubMailScrub
{
    class Program
    {
        static private Email[] grubhuborders;
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("incorrect usage");
                return;
            }
            if (!args[0].Contains(".mbox"))
            {
                Console.WriteLine("File must be an .mbox");
                return;
            }
            var parser = new Parser(args[0]);

            //var parser = new Parser("C:/Users/Brian/Documents/visual studio 2015/Projects/GrubHubMailScrub/GrubHubMailScrub/bin/Debug/download.mbox");
            parser.splitemailstostring();
            parser.createemails();
            parser.findgrubhuborders();
            parser.sortemails("Date");
            parser.conditional("Date");

            parser.print("Total");
        }
    }
}
