using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Data.Repo;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebUI.Hubs
{
	[HubName("Geo")]
	public class GeoHub : Hub
	{
		public void Hello()
		{
			Clients.All.hello();
		}

		public void SendLocation(GeoIp ip)
		{
			// Call the addNewMessageToPage method to update clients.
			Clients.All.SendLocation(ip);
		}
	}
}