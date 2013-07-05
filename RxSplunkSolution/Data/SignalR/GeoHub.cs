using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repo;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Data.SignalR
{
	public class GeoHub
	{
		private static GeoHub _hub;
		public static GeoHub GetGeoHub() {
			if(_hub == null)
				_hub = new GeoHub();
			return _hub;
		}


		public void SendMessage(GeoIp ip) {
			hubProxy.Invoke("SendLocation", ip).Wait();
		}

		public static void SendIp(GeoIp ip)
		{
			GetGeoHub().SendMessage(ip);
		}
	

		//Todo: Place in config file
		private string connectionUrl = "http://localhost:83";
		private string hubName = "Geo";


		public HubConnection connection { get; set; }
		public IHubProxy hubProxy { get; set; }


		

		/// <summary>
		/// Constructor. Initializes hubconnection
		/// </summary>
		public GeoHub() {
			connection = new HubConnection(connectionUrl);
			hubProxy = connection.CreateHubProxy(hubName);
			connection.Start().Wait();
			//hubProxy.Invoke("Send", "Info", "").Wait();
			_hub = this;
		}

	}
}
