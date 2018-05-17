using GrubHubMailScrub;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCrawler.Objects
{
    public class EmailCollection
    {
        private string filelocation;
        private string[] emailclumps;
        private Email[] emails;
        private Email[] grubhubemails;
        private FileStream fs;
        private string frompattern = "From \\S{23} \\S{3} \\S{3} \\S{2} \\S{8} \\S{5} \\S{4}";
        private string conditionalMessage;
        private Parser parser;

        public EmailCollection(string file)
        {
            filelocation = file;

        }

        public void SetupEmails()
        {
            parser = new Parser(filelocation);
            emailclumps = parser.splitemailstostring();
            emails = parser.createemails(emailclumps);
            
        }

        public void FindGrubHubEmails()
        {
            grubhubemails = parser.findgrubhuborders(emails);
        }

        #region printing
        public void print(string key)
        {
            Console.WriteLine(conditionalMessage);
            switch (key)
            {
                case "Order":
                    int count = 1;
                    foreach (Email e in grubhubemails)
                    {
                        Console.WriteLine($"Order Number {count}");
                        Console.WriteLine(e.toString());
                        count++;
                    }
                    break;
                case "Rest":

                    break;
                case "Total":
                    int count2 = 1;
                    var totalcost = 0.0;
                    var numberofrest = new Dictionary<string, int>();
                    foreach (Email e in grubhubemails)
                    {
                        totalcost += e.ordertotalnum;
                        try
                        {
                            numberofrest.Add(e.rest, 1);
                        }
                        catch (Exception ex)
                        {

                        }
                        count2++;
                    }
                    Console.WriteLine($"Numberof Orders {count2}");
                    Console.WriteLine($"Total Cost:{totalcost}");
                    Console.WriteLine($"Number of Dif Rest:{numberofrest.Count}");
                    break;
                default:
                    Console.WriteLine("InvalidPrint Key");
                    throw new InvalidOperationException();
                    break;

            }
        }
        #endregion

        #region conditionals
        public void conditional(string key)
        {
            switch (key)
            {
                case "Date":
                    var temp = from a in grubhubemails
                               where (a.datetime.CompareTo(new DateTime(2018, 1, 1)) > 0)
                               select a;
                    grubhubemails = temp.ToArray();
                    conditionalMessage = "All Orders Ordered between 1-1-2018 and Now";
                    break;
                default:
                    conditionalMessage = "Showing all orders";
                    break;
            }
        }


        #endregion

        #region sorting
        public void sortemails(string key)
        {
            switch (key)
            {
                case "Date":
                    Array.Sort(grubhubemails, delegate (Email x, Email y) { return x.datetime.CompareTo(y.datetime); }); ;
                    break;
                case "Rest":
                    Array.Sort(grubhubemails, delegate (Email x, Email y) { return x.rest.CompareTo(y.rest); });
                    break;
                case "Total":
                    Array.Sort(grubhubemails, delegate (Email x, Email y) { return x.ordertotalnum.CompareTo(y.ordertotalnum); });
                    break;
                default:
                    Console.WriteLine("Invalid Sort Key");
                    throw new InvalidOperationException();
                    break;
            }
        }
        #endregion

    }
}
