<Query Kind="Program">
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{

	var file = new RepoApacheLogLine().LogLines.ToObservable();
	var lines = System.IO.File.ReadAllLines("access_log.txt").ToObservable();
	
	var res = file.Take(10);
	
	var count = 0;

	using (res.Subscribe(
			logs => {
				Console.WriteLine(logs);
				count++;
			},
				error => {
			}, () => {Console.WriteLine(count++);}
		)
	)
	
	Console.ReadLine();
}

public class RepoApacheLogLine
    {
        public string DateTimeFormat { get; set; }

        public RepoApacheLogLine()
        {
            DateTimeFormat = "[dd/MMM/yyyy:HH:mm:ss+0200]";
            LogLines = System.IO.File.ReadAllLines("access_log.txt").Select(o => ParseLine(o));
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
            result.Command = lines[5];
            result.Url = lines[6];
            result.Status = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), lines[8]);
            result.Time = int.Parse(lines[9]);
            result.UserAgent = lines[11];
            return result;
        }
    }
	
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
