<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference Relative="..\RxSplunkSolution\Data\bin\Debug\Data.dll">D:\Programação\Source Code\ReactiveProgrammingRx\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.DataVisualization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <NuGetReference>Linq2Charts</NuGetReference>
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>Data.Model</Namespace>
  <Namespace>Data.Repo</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Linq.Charting</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
  <Namespace>System.Runtime.CompilerServices</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

void Main()
{
	
	// Create chart
	var columns = new Column{ Points = {}, LegendText = "Apache Log" };
	var chart = new Chart
	{ ChartAreas = { new ChartArea { Series = { columns }} }
	, Dock = DockStyle.Fill,
	};
	chart.Dump("Apache Log");
	var timestampNow = DateTime.Now;
	// Get information from log
	var apacheList = new RepoApacheLogLine("access_log.txt");
	var timeGeneratedApacheList = apacheList.GetObservableLogLines(1L);

	var test = timeGeneratedApacheList
	.LogTimestampedValues(o => {
		Console.WriteLine("Got a logline on "+(o.Timestamp - timestampNow).TotalSeconds);
		timestampNow = DateTime.Now;
	});
	//.Throttle(TimeSpan.FromMilliseconds(1000000)).Subscribe(o => Debug.WriteLine("Error: No request after logline: "+o));

	test.Subscribe();

	var graphRes = from window in timeGeneratedApacheList.Window(TimeSpan.FromSeconds(1))
				from stats in
                  (   // calculate statistics within one window
                      from line in window
                      group line by line.IP into g
                      from Count in g.Count()
                      select new
                      {
                          g.Key,
                          Count
                      }).ToList()
              select new { 
			  		stats.Count, 
					Points=from s in stats orderby s.Count descending 
					       select new { s.Count, Address = s.Key }
				};
	
	var count = 0;
	
	graphRes.Subscribe(
		lines => {
			chart.BeginInit(); 
			columns.BasePoints.Clear();
			count++;
			foreach(var point in lines.Points) columns.Add(point.Address, point.Count);
			//columns.Add("Windows Rcv",lines.Count);
			chart.EndInit(); }
	);
}
	
	public static class Utils {
	public static IObservable<T> LogTimestampedValues<T>(this IObservable<T> source, Action<Timestamped<T>> onNext)
        {
            return source.Timestamp().Do(onNext).Select(x => x.Value);
        }

	}

// Define other methods and classes here