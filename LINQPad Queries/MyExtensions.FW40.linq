<Query Kind="Program" />

void Main()
{
	// Write code to test your extensions here. Press F5 to compile and run.
}

public static class MyExtensions
{
	// Write custom extension methods here. They will be available to all queries.
	public class RepoApacheLogLine
    {
        public string DateTimeFormat { get; set; }

        public RepoApacheLogLine()
        {
            DateTimeFormat = "[dd/MMM/yyyy:HH:mm:ss+0200]";
            LogLines = System.IO.File.ReadAllLines("Files/access_log.txt").Select(o => ParseLine(o));
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
	
}

// You can also define non-static classes, enums, etc.