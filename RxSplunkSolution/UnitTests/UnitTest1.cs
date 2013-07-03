using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Model;
using System.Net;
using Data.Repo;
using System.Globalization;
using System.Threading;
using System.Linq;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Joins;
using System.Reactive.Linq;
using System.Reactive.PlatformServices;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using Core.SignalR;

namespace UnitTests
{
	[TestClass]
	public class TestRepoApacheLogLine
	{
		private ApacheLogLine LogLine {get;set;}
		private string LogLineString { get; set; }

		[TestMethod]
		public void TestParseLine()
		{
			var repo = new RepoApacheLogLine();
			var parsed = repo.ParseLine(LogLineString);
			Assert.AreEqual(LogLine, parsed);
		}

		[TestMethod]
		public void TestGetObservable()
		{
			Debug.WriteLine("Started reading apache loglines");
			var repo = new RepoApacheLogLine();
			var observable = repo.GetObservableLogLines(1);
			observable.Subscribe(o => Debug.WriteLine(o));
			Thread.Sleep(100000);
		}

		[TestInitialize()]
		public void Init()
		{
			LogLineString = "83.86.171.177 - - [15/May/2013:04:03:01 +0200] \"GET /weer/turfWeer HTTP/1.1\" 200 499 \"-\" \"Java/1.7.0_05\"";
			LogLine = new ApacheLogLine()
			{
				IP = "83.86.171.177",
				Date = DateTime.ParseExact("[15/May/2013:04:03:01]", "[dd/MMM/yyyy:HH:mm:ss]", CultureInfo.InvariantCulture),
				Command = "GET /weer/turfWeer HTTP/1.1",
				Status = HttpStatusCode.OK,
				Time = 499,
				UserAgent = "Java/1.7.0_05"
			};

		}

		[TestMethod]
		public void TestEF()
		{
			using (var db = new RepoEF())
			{
                var geoIp = new GeoIp() { Ip = "66.249.76.78", Lat = 37, Long = -122 };
				db.GeoIps.Add(geoIp);
				db.SaveChanges();

				// Display all Blogs from the database
				var query = from b in db.GeoIps
							orderby b.Ip
							select b;

				Debug.WriteLine("All geoips in the database:");
				foreach (var item in query)
				{
					Debug.WriteLine(item.Ip);
				}
			}
			Debug.WriteLine("Done...");
		}

		/// <summary>
		/// Tests the working of the consolehub by sending a message
		/// </summary>
		[TestMethod]
		public void TestConsoleHub()
		{
			var hub = new ConsoleHub();
			Thread.Sleep(1);
		}

        [TestMethod]
        public void EFGetAsync()
        {
            using (var db = new RepoEF())
            {
                var position = db.get("83.86.171.177");
                Debug.WriteLine("Position: " + position);
            }
        }

        [TestMethod]
        public void EFGet()
        {
            using (var db = new RepoEF())
            {
                var position = db.get("83.86.171.177");
                Debug.WriteLine(position.Lat+" "+position.Long);
            }
        }

        [TestMethod]
        public void IPInfo()
        {
            var geoIP = new FreeGeoIP();
            var location = geoIP.getIPLocation2("123.123.123.123");
            Debug.WriteLine("Location: " + location.Lat + " : " + location.Long);
        }

        [TestMethod]
        public void IPInfoObservable()
        {
            var geoIP = new FreeGeoIP();
            var location = geoIP.getIPLocation3("123.123.123.123");

            var ip = location.Wait();
            
            Debug.WriteLine("Location: " + ip.Lat + " : " + ip.Long);
           
        }

	}
}
