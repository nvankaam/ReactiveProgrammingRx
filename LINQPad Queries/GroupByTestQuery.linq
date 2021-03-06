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
	var columns = new Column{ Points = {}, LegendText = "Apache Log" };
	var chart = new Chart
	{ ChartAreas = { new ChartArea { Series = { columns }} }
	, Dock = DockStyle.Fill,
	};
	chart.Dump("Apache Log");

	var apacheList = new RepoApacheLogLine("access_log.txt").LogLines.Take(100).ToList();
	Utils.Shuffle<ApacheLogLine>(apacheList);
	var apacheListObservable = apacheList.ToObservable();
	
	var apacheObservable = Observable.Create<ApacheLogLine>(o =>
	{
		var els = new EventLoopScheduler();
		return apacheListObservable.ObserveOn(els)
		.Do(xx => els.Schedule(() => Thread.Sleep(10)))
		.Subscribe(o);
	}
	);
	//var apacheLines = apacheList.ToObservable();
	var timeInterval = new TimeSpan(0,15,0); //15 minutes
	var count = 0;
	
	var x = apacheObservable
			.GroupBy(
				o => {
				return o.Date.Ticks / timeInterval.Ticks;
				}
			);
	int i = 0;
			
	x.Subscribe(
		groups => {
				groups.Subscribe(line => Console.WriteLine("Date "+(i++)+" is: "+line.Date));
			});
	
x.ObserveOn(chart).Subscribe(v =>
{
	chart.BeginInit();
	
		columns.BasePoints.Clear();
		//foreach(var point in v.Points) columns.Add(point.Address, point.Count);
		
	chart.EndInit();
});
	

//	Console.ReadLine();
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