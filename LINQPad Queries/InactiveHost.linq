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
	//Some confiugration options
	var concurrentInterval = 60;
	var simulationSpeed = 3000L;
	var windowSize = (double)concurrentInterval/(double)simulationSpeed;

	// Get information from log
	var repository = new RepoApacheLogLine("access_log.txt");
	var timeGeneratedApacheList = repository.GetObservableLogLines(simulationSpeed);
	
	
//	timeGeneratedApacheList.DumpLive("grid");
	var ipObservers = timeGeneratedApacheList.GroupBy(line => line.IP);
	
	
	ipObservers.Subscribe(
		ip => {
			Console.WriteLine(String.Format("Got tuple: {0}", ip.Key));
			ip.Subscribe(o => Console.WriteLine("{0} updated on {1}", ip.Key, o));
		}
	);
	
	ipObservers.Select(group => {
		return Tuple.Create(group.Key, group.Latest().First().Date);
	}).Select(o => o.Item1).DumpLive("grid");
	
	
	
	ipObservers.Buffer(100).DumpLive();

//	ipObservers.Select(o => {
//		return Tuple.Create(o.Key, o.Last().Date).
//	}).
}

public static class Utils {

}

// Define other methods and classes here