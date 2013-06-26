<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference Relative="..\RxSplunkSolution\Data\bin\Debug\Data.dll">D:\Programação\Source Code\ReactiveProgrammingRx\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.DataVisualization.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <NuGetReference Prerelease="true">EntityFramework</NuGetReference>
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
	
	// Initialize DB component
	var db = new RepoEF();
    
	var timeGeneratedApacheList = apacheList.GetObservableLogLines(60L);

	Dictionary<string, int> class_count = new Dictionary<string, int>();

	// Make the IP matchings (right now it is unique IPs)
	var groups = timeGeneratedApacheList
						.GroupBy( lines => { return lines.IP; } );
		
	var geoGroups = groups.Select( g => 
		{
			return new { grp = g, geoIP = Utils.getGeoIP(g.Key)};
		}
	);
	
	var gr = geoGroups.SelectMany( geoGroup => {
						var geoLocation = geoGroup.geoIP;
						return geoGroup.grp.Select( line => new { line = line, geoIP = geoLocation });
					});
					
	gr.Subscribe( x => Console.WriteLine(x.geoIP+" "+x.line));
	
	// Subscribe to new classes being created
	/*uniqueIPs.Subscribe(
		lines => { 
			// Ascynchronous call to database
			// ...
			// Subscribe to matched objects into class
			class_count.Add(lines.Key,0);
			lines.Subscribe(plus_one => { 
				class_count[plus_one.IP] = class_count[plus_one.IP] + 1;
				class_count.Dump();
				//Console.WriteLine("   +1 to "+lines.Key+" with time "+plus_one.Date); // Count process
			}); 
			//Console.WriteLine("New Unique IP "+lines.Key);
		}
	);*/
/*
	var consoleRes = timeGeneratedApacheList.Window(TimeSpan.FromSeconds(5)).Subscribe(
		lines => { lines.ToList().Dump(); }
	);*/
	/*
	consoleRes.Subscribe(
		lines => { lines.ToList().Dump(); }
	);*/

	/*
	timeGeneratedApacheList.Subscribe(
		success => {
			Console.WriteLine(db.get(success.IP));
		}
	);
	*/
		/*		
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
	);*/
}

public static class Utils {
	public static IObservable<int> getGeoIP(string ip) {
			return Observable.Create<int>(observer => {
			Task.Delay(1000);
			observer.OnNext(6);
			observer.OnCompleted();
			return Disposable.Create(() => Console.WriteLine("Observer has unsubscribed"));
		});
	}
}