using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Data.SignalR
{
	public class ConsoleHub
	{
		private static ConsoleHub _hub;
		public static ConsoleHub GetConsoleHub() {
			if(_hub == null)
				_hub = new ConsoleHub();
			return _hub;
		}


		public void SendMessage(string category, string message) {
			hubProxy.Invoke("Send", category, message).Wait();
		}

		public static void LogApacheLogLine(ApacheLogLine line) {
			GetConsoleHub().SendMessage("LogLines", line.ToString());
		}

		//Todo: Place in config file
		private string connectionUrl = "http://localhost:83";
		private string hubName = "Console";


		public HubConnection connection { get; set; }
		public IHubProxy hubProxy { get; set; }


		

		/// <summary>
		/// Constructor. Initializes hubconnection
		/// </summary>
		public ConsoleHub() {
			connection = new HubConnection(connectionUrl);
			hubProxy = connection.CreateHubProxy(hubName);
			connection.Start().Wait();
			hubProxy.Invoke("Send", "Console", "Console connected to hub").Wait();
			_hub = this;
		}

	}
}
