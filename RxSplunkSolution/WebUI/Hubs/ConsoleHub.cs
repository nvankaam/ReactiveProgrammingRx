using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace WebUI.Infra
{
	[HubName("Console")] 
	public class ConsoleHub : Hub
	{
		public void Hello()
		{
			Clients.All.hello();
		}

		public void Send(string name, string message)
		{
			// Call the addNewMessageToPage method to update clients.
			Clients.All.addNewMessageToPage(name, message);
		}
	}
}