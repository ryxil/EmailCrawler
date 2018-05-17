using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EmailCrawler.Objects;

namespace GrubHubMailScrub
{
    public class Start
    {
        static void Main(string[] args)
        {

            if (args.Length != 1)
            {
                Console.WriteLine("**.exe {Compressed fileshare of emails}");
                return;
            }

            var EmailCollection = new EmailCollection(args[0]);

            EmailCollection = new EmailCollection("C:/Users/Brian/Documents/visual studio 2015/Projects/GrubHubMailScrub/GrubHubMailScrub/bin/Debug/download.mbox");
            EmailCollection.SetupEmails();
            EmailCollection.FindGrubHubEmails();

            String readinline = "";
            do
            {
                try
                {
                    Console.WriteLine("How do you want to Sort the Emails");
                    readinline = Console.ReadLine();
                    EmailCollection.sortemails(readinline);

                    Console.WriteLine("Add Conditionals");
                    readinline = Console.ReadLine();
                    EmailCollection.conditional(readinline);

                    Console.WriteLine("How should it be printed");
                    readinline = Console.ReadLine();
                    EmailCollection.print(readinline);

                }
                catch(InvalidOperationException ex)
                {
                    Console.WriteLine("Invalid Entree");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("shit broke");
                }
                Console.WriteLine("Hit Enter to Exit...");
                readinline = Console.ReadLine();
            } while (!readinline.Equals(""));


        }
    }
}
