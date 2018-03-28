using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Communication
{
    public interface ICommunication : IDisposable
    {
        IHubConnectionContext<dynamic> Clients
        {
            get; set;
        }
    }
}
