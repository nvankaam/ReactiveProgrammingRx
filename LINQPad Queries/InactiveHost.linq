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
	//Some confiugration options
	var updateTime = 60;
	var simulationSpeed = 600L;
	var windowSize = (double)updateTime/(double)simulationSpeed;

	// Get information from log
	var repository = new RepoApacheLogLine("access_log.txt");
	var timeGeneratedApacheList = repository.GetObservableLogLines(simulationSpeed);
	
	
	var observable = Observable.Create<IEnumerable<KeyValuePair<string, DateTime>>>((observer) => {
		var stringDict = new Dictionary<string, DateTime>();
		var value = "";
		return timeGeneratedApacheList.GroupBy(line => line.IP).SelectMany(grp => 
			grp.Select(o => Tuple.Create(o.IP, o.Date))
		).Subscribe(o2 => {
			value = o2.Item1;
			stringDict.Remove(o2.Item1);
			stringDict.Add(o2.Item1, o2.Item2);
			observer.OnNext(stringDict.OrderBy(o3 => o3.Value));
		});
	});
	
	//Visualisation with dumplive
	observable.Select(o => {
		var result = "";
		foreach (var data in o) {
			result += data.Key+"-"+data.Value+"\r\n";
		}
		return result;
	}).DumpLive();
	
// Define other methods and classes here
}