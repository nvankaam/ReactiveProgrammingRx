using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Core.SignalR
{
	public class ConsoleHub
	{
		//Todo: Place in config file
		private string connectionUrl = "http://localhost:83";
		private string hubName = "chat";


		public HubConnection connection { get; set; }
		public IHubProxy hubProxe { get; set; }


		/// <summary>
		/// Constructor. Initializes hubconnection
		/// </summary>
		public ConsoleHub() {
			connection = new HubConnection(connectionUrl);
			IHubProxy consoleProxy = connection.CreateHubProxy(hubName);
			connection.Start().Wait();
			consoleProxy.Invoke("Send", "Server", "HI!").Wait();
		
		}
	}
}
