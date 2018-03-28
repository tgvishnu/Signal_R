using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Hubs;
using Vishnu.Messenger.Common;
using System.Diagnostics;

namespace Vishnu.Messenger.Core.Communication
{
    public class Broadcast : ICommunication
    {
        private static readonly List<Common.Models.Message> _currentMessages = new List<Common.Models.Message>();
        private static Broadcast _instance = null;
        private static object _locker = new object();

        private Broadcast(IHubConnectionContext<dynamic> clients)
        {
            this.Clients = clients;
            Debug.WriteLine("Broadcast Constructor");
        }

        public IHubConnectionContext<dynamic> Clients
        {
            get; set;
        }

        public static Broadcast Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_locker)
                    {
                        if(_instance == null)
                        {
                            _instance = new Broadcast(GlobalHost.ConnectionManager.GetHubContext<MessageHub>().Clients);
                        }
                    }
                }

                return _instance;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                try
                {
                    if (disposing)
                    {
                        if (_currentMessages != null)
                        {
                            _currentMessages.Clear();
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
