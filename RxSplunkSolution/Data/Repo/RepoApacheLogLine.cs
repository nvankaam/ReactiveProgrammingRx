using Data.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Diagnostics;


namespace Data.Repo
{
    public class RepoApacheLogLine
    {
        public string DateTimeFormat { get; set; }

        public RepoApacheLogLine(string filename = "..\\..\\..\\Data\\Files\\access_log.txt")
        {
            DateTimeFormat = "[dd/MMM/yyyy:HH:mm:ss+0200]";
            LogLines = System.IO.File.ReadAllLines(filename).Select(o => ParseLine(o));
        }

        public IEnumerable<ApacheLogLine> LogLines{ get; set; }

        public IObservable<ApacheLogLine> GetObservableLogLines(Int64 speed)
        {
            var lastTime = LogLines.First().Date;
            var enumerator = LogLines.GetEnumerator();

            var observable = Observable.Generate(
                    enumerator,
                    value => value.MoveNext(),
                    value => value,
                    value => value.Current,
                    value => {
                        var result = TimeSpan.FromMilliseconds((value.Current.Date - lastTime).TotalMilliseconds / speed);
                        lastTime = value.Current.Date;
                        return result;
                    });

            observable.Subscribe(o => 
                Debug.WriteLine(o)
                );

            return observable;
        }
        
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
            result.IP = lines[0];
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
