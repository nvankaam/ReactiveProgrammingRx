<Query Kind="Statements">
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

//Some confiugration options
var concurrentInterval = 60;
var simulationSpeed = 300L;
var windowSize = (double)concurrentInterval/(double)simulationSpeed;

//Create the chart
var columns = new Column{ Points = {}, LegendText = "Apache Log Concurrent Users" };
var chart = new Chart
{ ChartAreas = { new ChartArea { Series = { columns }} }
, Dock = DockStyle.Bottom,
};
chart.Dump("Apache Log");

// Get information from log
var repository = new RepoApacheLogLine("access_log.txt");
var timeGeneratedApacheList = repository.GetObservableLogLines(simulationSpeed);


//Generate a observable that contains both the request, and the amount of concurrent users at the moment of the request
//Note this has sort of a state because of the list in here. I do not know how to do this
//in an efficient way without keeping a state.
//TODO: Think about a better solution
IList<ApacheLogLine> concurentList = new List<ApacheLogLine>();
var concurentObservable = timeGeneratedApacheList.Select(line => {
	concurentList.Add(line);
	concurentList = concurentList.Where(o => o.Date.AddSeconds(concurrentInterval) > line.Date).ToList();
	return new ActiveUserCount(){Moment = line.Date, Count = concurentList.Count()};
});

//Create the windows containing the concurent users over a time interval. 
//TODO: use something else for the window size so it is not a state
//TODO: Make sure all timespans are windows, also when there is no data for that window!
var totalWindows = 0;
var windowObservable = concurentObservable.Window(TimeSpan.FromSeconds(windowSize)).SelectMany(window => {
	totalWindows++;
	var windowName = "Window"+totalWindows;
	return window.Max(o => o.Count).Select(maxValue => {
		return new ActiveUserCount{ Count = maxValue, WindowName = windowName };
		
	});		
});

//Create another window over the other windows, so we have multiple bars in the graph
var graphObservable = windowObservable.Window(10, 1).SelectMany(graphWindow => {
	return graphWindow.ToList().Select(actionList => new GraphPoint() {Points=actionList});
});
	
//Subscribe the graph to the data
//TODO: Fix the graph so it has a fixed scale (this is annoying)
graphObservable.Subscribe(
	window => {
		chart.BeginInit(); 
		columns.BasePoints.Clear();
		foreach(var point in window.Points) columns.Add(point.WindowName, point.Count);
		chart.EndInit(); 
	});
}

//Filters the old dates out of the time. <3 linq
public static IEnumerable<ApacheLogLine> RemoveOld(IEnumerable<ApacheLogLine> input,DateTime currentTime, int interval) {
return input.Where(o => o.Date.AddSeconds(interval) < currentTime);
}

//Data classes
public class ActiveUserCount {
    public DateTime Moment { get; set; }
    public int Count { get; set; }
public string WindowName {get; set;}
}


public class GraphPoint {
public IList<ActiveUserCount> Points { get; set;}
}

public static class Utils {