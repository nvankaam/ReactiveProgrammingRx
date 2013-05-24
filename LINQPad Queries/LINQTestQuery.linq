<Query Kind="Program">
  <Reference Relative="..\RxSplunkSolution\Data\bin\Debug\Data.dll">&lt;MyDocuments&gt;\Studie\Reactive\Repo\RxSplunkSolution\Data\bin\Debug\Data.dll</Reference>
  <NuGetReference>Rx-Main</NuGetReference>
  <Namespace>Data.Model</Namespace>
  <Namespace>Data.Repo</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections</Namespace>
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
	var apacheList = new RepoApacheLogLine("access_log.txt").LogLines.ToList();
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
			).SelectMany(o => o.FirstAsync());
			
			
	x.Subscribe(
			logs => {
				Console.WriteLine(logs.Date +" - "+ DateTime.Now.Millisecond);
				Console.WriteLine(count++);
			},
			error => {Console.WriteLine("ERROR");}, () => {Console.WriteLine("Count: "+count);}
		);

	

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