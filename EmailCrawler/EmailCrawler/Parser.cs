using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GrubHubMailScrub
{
    class Parser
    {
        private string filelocation;
        private string[] emailclumps;
        private Email[] emails;
        private Email[] grubhubemails;
        private Email[] sortedemails;
        private Email[] conditionEmails;
        private FileStream fs;
        private string frompattern = "From \\S{23} \\S{3} \\S{3} \\S{2} \\S{8} \\S{5} \\S{4}";
        private string conditionalMessage;

        public Parser(string file)
        {
            filelocation = file;
        }
        #region parse and create email list
        public void splitemailstostring()
        {
            System.IO.StreamReader file;
            try
            {
                file = new System.IO.StreamReader(filelocation);
            }
            catch (Exception ex)
            {
                return;
            }
            List<string> chuncks = new List<string>();


            int count = 0;
            String line;
            StringBuilder sb = new StringBuilder();
            while ((line = file.ReadLine()) != null)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(line, frompattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    chuncks.Add(sb.ToString());
                    count++;
                    sb.Clear();
                }
                sb.AppendLine(line);
            }
            emailclumps = chuncks.ToArray();
        }


        public void createemails()
        {
            List<Email> emaillist = new List<Email>();
            foreach (string s in emailclumps)
            {
                if (!s.Equals(""))
                {
                    var email = new Email(s);
                    email.parse();

                    emaillist.Add(email);
                }
            }
            emails = emaillist.ToArray();

        }

        public void findgrubhuborders()
        {
            List<Email> list = new List<Email>();
            foreach (Email e in emails)
            {
                if (e.isgrubhuborder())
                {
                    e.cleanupfields();
                    e.setrest();
                    e.settotal();
                    if (e.isvalidorder())
                    {
                        list.Add(e);
                    }

                }
            }
            grubhubemails = list.ToArray();
        }
        #endregion

        #region printing
        public void print(string key)
        {
            Console.WriteLine(conditionalMessage);
            switch (key)
            {
                case "Order":
                    int count = 1;
                    foreach (Email e in conditionEmails)
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
                    foreach (Email e in conditionEmails)
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

            }
        }
        #endregion

        #region conditionals
        public void conditional(string key)
        {
            switch (key)
            {
                case "Date":
                    var temp = from a in sortedemails
                               where (a.datetime.CompareTo(new DateTime(2018, 1, 1)) > 0)
                               select a;
                    conditionEmails = temp.ToArray();
                    conditionalMessage = "All Orders Ordered between 1-1-2018 and Now";
                    break;
                default:
                    conditionEmails = sortedemails;
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
                    sortedemails = sortbyDate(grubhubemails);
                    break;
                case "Rest":
                    sortedemails = sortbyRest(grubhubemails);
                    break;
                case "Total":
                    sortedemails = sortbyTotal(grubhubemails);
                    break;
                default:
                    Console.WriteLine("Invalid Sort Key");
                    break;
            }
        }

        public Email[] sortbyDate(Email[] e)
        {
            var ret = e;
            Array.Sort(ret, delegate (Email x, Email y) { return x.datetime.CompareTo(y.datetime); });
            return ret;
        }
        public Email[] sortbyRest(Email[] e)
        {
            var ret = e;
            Array.Sort(ret, delegate (Email x, Email y) { return x.rest.CompareTo(y.rest); });
            return ret;
        }
        public Email[] sortbyTotal(Email[] e)
        {
            var ret = e;
            Array.Sort(ret, delegate (Email x, Email y) { return x.ordertotalnum.CompareTo(y.ordertotalnum); });
            return ret;
        }
        #endregion
    }
}
