<Query Kind="Program">
  <Reference Relative="..\RxSplunkSolution\Data\bin\Debug\Data.dll">&lt;MyDocuments&gt;\Studie\Reactive\Repo\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>Data.Model</Namespace>
  <Namespace>Data.Repo</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{

	var apacheLines = new RepoApacheLogLine("access_log.txt").LogLines.ToObservable();
	
	var timeInterval = new TimeSpan(0,15,0); //15 minutes
	
	var result = apacheLines
		.Select(
		o => new {
			o.GroupBy(
				o => o.Date.Ticks / timeInterval.Ticks
			).Select(
				o => o.Take(1)
			)
		})
		};
		

//
//	apacheLines.Subscribe(
//			logs => {
//				Console.WriteLine(logs);
//				count++;
//			},
//				error => {
//			}, () => {Console.WriteLine(count++);}
//		);
//	
//	
//	Console.ReadLine();
}

// Define other methods and classes here