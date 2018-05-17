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

        public Parser(string file)
        {
            filelocation = file;
        }


        public string[] splitemailstostring()
        {
            System.IO.StreamReader file;
            try
            {
                file = new System.IO.StreamReader(filelocation);
            }
            catch (Exception ex)
            {
                return null;
            }
            List<string> chuncks = new List<string>();


            int count = 0;
            String line;
            string frompattern = "From \\S{23} \\S{3} \\S{3} \\S{2} \\S{8} \\S{5} \\S{4}";
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
            return chuncks.ToArray();
        }


        public Email[] createemails(string[] emailclumps)
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
            return emaillist.ToArray();

        }

        public Email[] findgrubhuborders(Email[] emails)
        {
            List<Email> list = new List<Email>();
            foreach (Email e in emails)
            {
                if (isgrubhuborder(e))
                {
                    e.cleanupfields();
                    e.setrest();
                    e.settotal();
                    if (isvalidorder(e))
                    {
                        list.Add(e);
                    }

                }
            }
            return list.ToArray();
        }

        public bool isgrubhuborder(Email e)
        {
            return e.Subject.Contains("Your Order from ");
        }

        public bool isvalidorder(Email e)
        {
            bool temp = true;
            if (e.Date == null) temp = false;
            if (e.datetime == null) temp = false;
            if (e.rest == null) temp = false;
            if (e.orderTotal == null) temp = false;
            return temp;
        }

    }
}
