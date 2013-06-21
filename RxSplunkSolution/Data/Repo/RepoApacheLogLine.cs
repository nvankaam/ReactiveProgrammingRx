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
using System.Reactive.Concurrency;
using System.Threading;


namespace Data.Repo
{
	public class RepoApacheLogLine
	{
		public string DateTimeFormat { get; set; }
		private int Chunksize = 10000;

		public RepoApacheLogLine(string filename = "..\\..\\..\\Data\\Files\\access_log.txt")
		{
			DateTimeFormat = "[dd/MMM/yyyy:HH:mm:ss+0200]";
			LogLines = System.IO.File.ReadAllLines(filename).Select(o => ParseLine(o));
		}

		public IEnumerable<ApacheLogLine> LogLines{ get; set; }

		public IObservable<ApacheLogLine> GetObservableLogLines(Int64 speed)
		{
			var beginTime = LogLines.First().Date;
			var lines = LogLines.ToList();
			var enumerator = LogLines.GetEnumerator();
			var numberOfChunks = lines.Count / Chunksize;

			//Fix from http://stackoverflow.com/questions/13462713/why-does-observable-generate-throw-system-stackoverflowexception
			//To prevent stackoverflow due to recursive implementation of Observable.generate
			//Basically Observable.generate is called multiple times  for each chunk (where a chunk contains 10000 lines)
			var time = new HistoricalScheduler(beginTime);
			var streams =
			from chunkIndex in Enumerable.Range(0, (int)Math.Ceiling((double)lines.Count / Chunksize) - 1)
			let startIdx = chunkIndex * Chunksize
			let endIdx = Math.Min(lines.Count, startIdx + Chunksize)
			select Observable.Generate(
				startIdx,
				s => s < endIdx,
				s => s + 1,
				s => lines[s],
				s => lines[s].Date,
				time
				);

			//Merge all streems
			var stream = Observable.Concat(streams);

			//Run the simulation in a seperate thread
			SimulateLogLines(time, lines, speed);
		
			//Dirty fix to allow multiple subscribers.
			//As this is just for testing purposes it is ok.
			//var subj = new Subject<ApacheLogLine>();
			//stream.Subscribe(tsst => subj.OnNext(tsst));
			return stream;
		}
		
		/// <summary>
		/// Simulates the time of the given loglines on the timer with the given speed
		/// runs the simulation on a different thread so this method does not block
		/// </summary>
		/// <param name="timer"></param>
		/// <param name="lines"></param>
		private void SimulateLogLines(HistoricalScheduler scheduler, List<ApacheLogLine> logLines, long speed) {

			Task t1 = new Task(() =>
			{
				var lastTime = logLines.First().Date;
				logLines.ForEach(logLine =>
				{
					var totalSeconds = (logLine.Date - lastTime).TotalMilliseconds;
					if (totalSeconds < 0)
						totalSeconds = 0; //Sometimes it seems not all lines are in the correct order
					var interval = TimeSpan.FromMilliseconds(totalSeconds);
					var result = TimeSpan.FromMilliseconds(totalSeconds / (double)speed);
					lastTime = logLine.Date;
					Thread.Sleep(result);
					scheduler.AdvanceBy(interval);
				});
			});
			t1.Start();

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
