using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace api.Hubs
{
    [HubName("ServerAtosCapital")]
    public class ServerHub : Hub
    {
        public void Conectado(string token)
        {
            Negocios.SignalR.GatewayMonitorCargas.enviaLista(Context.ConnectionId);
        }

        //public void Send(String nome, String message)
        //{
        //    // Call the addNewMessageToPage method to update clients.
        //    Clients.All.addNewMessageToPage(nome, message);
        //}

        public static void Show()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ServerHub>();
            context.Clients.All.displayStatus();
        }
    }
}