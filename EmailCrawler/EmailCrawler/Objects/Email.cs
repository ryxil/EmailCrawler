using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrubHubMailScrub
{
    class Email
    {
        private string emailclump;

        public DateTime datetime;
        public string Date;
        private string Datepattern = "Date: ";
        public string From;
        private string Frompattern = "From: ";
        public string Subject;
        private string Subjectpattern = "Subject: ";
        public string Body;
        private string Bodypattern = "Message-ID: ";

        public string rest;
        public string orderTotal;
        public double ordertotalnum;


        public Email(string ec)
        {
            emailclump = ec;
        }

        public void parse()
        {
            var lines = emailclump.Split('\n');
            StringBuilder sb = new StringBuilder();
            bool bodyflag = false;
            string s;
            foreach (string st in lines)
            {
                if (st.Length != 0)
                    s = st.Substring(0, st.Length - 1);
                else
                    s = "";
                if (System.Text.RegularExpressions.Regex.IsMatch(s, Datepattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    Date = s;
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(s, Frompattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    From = s;
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(s, Subjectpattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    Subject = s;
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(s, Bodypattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    sb.AppendLine(s);
                    bodyflag = true;
                }
                if (bodyflag)
                {
                    sb.AppendLine(s);
                }
            }
            Body = sb.ToString();
        }

        public void cleanupfields()
        {
            this.Date = this.Date.Substring(5, this.Date.Length - 5);
            try
            {
                this.datetime = DateTime.Parse(this.Date.Substring(5, this.Date.Length - 12));
            }
            catch (Exception ex) { }
            this.From = this.From.Substring(5, this.From.Length - 5);
            this.Subject = this.Subject.Substring(8, this.Subject.Length - 8);
            this.Body = this.Body.Substring(11, this.Body.Length - 11);
        }

        public void setrest()
        {
            var beg = "Your Order from ";
            var end = "Is Being Prepared";
            this.rest = this.Subject.Substring(beg.Length, (this.Subject.Length - beg.Length - end.Length));
        }

        public void settotal()
        {
            var beg = "<b>$";
            var end = "</b>";
            var begindex = this.emailclump.IndexOf(beg);
            if (begindex != -1)
            {
                var enddex = this.emailclump.IndexOf(end, begindex);
                this.orderTotal = this.emailclump.Substring(begindex + beg.Length - 1, (enddex - begindex) - end.Length + 1);
                Double.TryParse(this.orderTotal.Substring(1, this.orderTotal.Length - 1), out this.ordertotalnum);
            }
        }

        public string toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Date:{this.datetime.ToString()}");
            sb.AppendLine($"Resturant:{this.rest}");
            sb.AppendLine($"OrderTotal:${this.ordertotalnum}");
            return sb.ToString();
        }
    }
}
