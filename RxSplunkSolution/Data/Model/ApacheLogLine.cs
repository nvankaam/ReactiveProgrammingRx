using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class ApacheLogLine
    {
        public string OriginalLine { get; set; }
        public IPAddress IP { get; set; }
        public DateTime Date { get; set; }
        //TODO: Parse aswell
        public String Command { get; set; }
        public HttpStatusCode Status { get; set; }
        public int Time { get; set; }

        public string Url { get; set; }
        public string UserAgent { get; set; }

        public override string ToString()
        {
            return OriginalLine;
        }
    }
}
