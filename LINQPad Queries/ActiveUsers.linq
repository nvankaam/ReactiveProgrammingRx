<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference Relative="..\RxSplunkSolution\Data\bin\Debug\Data.dll">&lt;MyDocuments&gt;\Studie\Reactive\Repo\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
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
	var concurrentInterval = 60;
	var windowSize = 0.2;
	
	// Create chart
	var columns = new Column{ Points = {}, LegendText = "Apache Log Concurrent Users" };
	var chart = new Chart
	{ ChartAreas = { new ChartArea { Series = { columns }} }
	, Dock = DockStyle.Bottom,
	};
	chart.Dump("Apache Log");

	// Get information from log
	var apacheList = new RepoApacheLogLine("access_log.txt");
	var timeGeneratedApacheList = apacheList.GetObservableLogLines(300L);
/*
	var consoleRes = timeGeneratedApacheList.Window(TimeSpan.FromSeconds(5));
	
	consoleRes.Subscribe(
		lines => { lines.ToList().Dump(); }
	);*/


	//var x = new {Date = o.Date, Count = concurentList.Count()};
	
//	var concurentSubject = new Subject<ActiveUserCount>();
	IList<ApacheLogLine> concurentList = new List<ApacheLogLine>();
	var concurentObservable = timeGeneratedApacheList.Select(line => {
		concurentList.Add(line);
		concurentList = concurentList.Where(o => o.Date.AddSeconds(concurrentInterval) > line.Date).ToList();
		return new ActiveUserCount(){Moment = line.Date, Count = concurentList.Count()};
	});
	
//	var subscription = timeGeneratedApacheList.Subscribe(
//		line => {
//			concurentList.Add(line);
//			concurentList = concurentList.Where(o => o.Date.AddSeconds(concurrentInterval) > line.Date).ToList();
//			concurentSubject.OnNext(new ActiveUserCount(){Moment = line.Date, Count = concurentList.Count()});
//		}
//	);
	

//	var totalWindows = 0;
//	timeGeneratedApacheList.Window(TimeSpan.FromSeconds(concurrentInterval)).Subscribe(
//		window =>  {
//			totalWindows++;
//			Console.WriteLine("Recieved a new window: "+totalWindows);
//			var windowName = "Window"+totalWindows;
//			window.Subscribe(
//				value => Console.WriteLine("{0} : {1}", windowName, value.OriginalLine),
//				ex => Console.WriteLine("{0} : {1}", windowName, ex),
//				() => Console.WriteLine("{0} Completed", windowName)
//			);
//		}
//		,
//		() => Console.WriteLine("Completed")
//	);
	
	
	var totalWindows = 0;
	
	var windowObservable = concurentObservable.Window(TimeSpan.FromSeconds(windowSize)).SelectMany(window => {
		totalWindows++;
//		Console.WriteLine("Created a new concurrent window: "+totalWindows);
		var windowName = "Window"+totalWindows;
//		window.Subscribe(
//			value => Console.WriteLine("{0} : {1} ({2})", windowName, value.Moment, value.Count),
//			ex => Console.WriteLine("{0} : {1}", windowName, ex),
//			() => Console.WriteLine("{0} Completed", windowName)
//		);
		return window.Max(o => o.Count).Select(maxValue => {
			return new ActiveUserCount{ Count = maxValue, WindowName = windowName };
			
		});		
	});
	
	var graphObservable = windowObservable.Window(10, 1).SelectMany(graphWindow => {
		return graphWindow.ToList().Select(actionList => new GraphPoint() {Points=actionList});
	});
	
	graphObservable.Subscribe(o => Console.WriteLine("Graphobs"));
	
	graphObservable.Subscribe(
		window => {
			Console.WriteLine("Drawing graph");
			chart.BeginInit(); 
			columns.BasePoints.Clear();
			//count++;
			foreach(var point in window.Points) columns.Add(point.WindowName, point.Count);
			//columns.Add("Windows Rcv",lines.Count);
			chart.EndInit(); 
		});
	

//	var max = from window in concurentSubject.Window()
//					select window.Where(o => o.Count == window.Max(o2 => o2.Count));
//	
//	
//						
//	max.Subscribe(o =>
//		o.Subscribe(o2 =>Console.WriteLine("Totaal aantal: {0} op", o2))
//		
//	);
				
//	
//	var count = 0;
//	
//	graphRes.Subscribe(
//		lines => {
//			chart.BeginInit(); 
//			columns.BasePoints.Clear();
//			count++;
//			foreach(var point in lines.Points) columns.Add(point., point.Count);
//			//columns.Add("Windows Rcv",lines.Count);
//			chart.EndInit(); }
//	);
}

//Filters the old dates out of the time. <3 linq
public static IEnumerable<ApacheLogLine> RemoveOld(IEnumerable<ApacheLogLine> input,DateTime currentTime, int interval) {
	return input.Where(o => o.Date.AddSeconds(interval) < currentTime);
}

public class ActiveUserCount {
    public DateTime Moment { get; set; }
    public int Count { get; set; }
	public string WindowName {get; set;}
}

public class GraphPoint {
	public IList<ActiveUserCount> Points {get; set;}
}

public static class Utils {
public static void Shuffle<T>(this IList<T> list)  
	{  
		Random rng = new Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

}

// Define other methods and classes here