<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;MyDocuments&gt;\Studie\Reactive\Repo\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
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
	
	// Create chart
	var columns = new Column{ Points = {}, LegendText = "Apache Log" };
	var chart = new Chart
	{ ChartAreas = { new ChartArea { Series = { columns }} }
	, Dock = DockStyle.Bottom,
	};
	chart.Dump("Apache Log");

	// Get information from log
	var apacheList = new RepoApacheLogLine("access_log.txt");
	var timeGeneratedApacheList = apacheList.GetObservableLogLines(60L);
/*
	var consoleRes = timeGeneratedApacheList.Window(TimeSpan.FromSeconds(5));
	
	consoleRes.Subscribe(
		lines => { lines.ToList().Dump(); }
	);*/

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