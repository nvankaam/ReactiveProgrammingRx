using Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repo
{
    public class RepoApacheLogLine
    {
        public string DateTimeFormat { get; set; }

        public RepoApacheLogLine(string filename = "..\\RxSplunkSolution\\Data\\Files\\access_log.txt")
        {
            DateTimeFormat = "[dd/MMM/yyyy:HH:mm:ss+0200]";
            LogLines = System.IO.File.ReadAllLines(filename).Select(o => ParseLine(o));
        }

        public IEnumerable<ApacheLogLine> LogLines{ get; set; }

        
        /// <summary>
        /// Parses a single line into a apacha log line class
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public ApacheLogLine ParseLine(string line)
        {
            var result = new ApacheLogLine();
            var lines = line.Split(' ');
            result.OriginalLine = line;
            result.IP = IPAddress.Parse(lines[0]);
            result.Date = DateTime.ParseExact(lines[3]+lines[4], DateTimeFormat, CultureInfo.InvariantCulture);
            result.Command = lines[5].Trim('"');
            result.Url = lines[6];
            result.Status = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), lines[8]);
            result.Time = int.Parse(lines[9]);
            result.UserAgent = lines[11].Trim('"');
            return result;
        }
    }
}
