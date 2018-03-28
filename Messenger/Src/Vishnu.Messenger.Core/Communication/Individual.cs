using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Hubs;

namespace Vishnu.Messenger.Core.Communication
{
    public class Individual : ICommunication
    {
        private static readonly Lazy<Individual> _instance = new Lazy<Individual>(() => new Individual(GlobalHost.ConnectionManager.GetHubContext<MessageHub>().Clients));

        public static Individual Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private Individual(IHubConnectionContext<dynamic> clients)
        {
            this.Clients = clients;
        }

        public IHubConnectionContext<dynamic> Clients
        {
            get; set;
        }

        //private void Send(Models.User toUser, Models.User fromUser, string message)
        //{
        //    if(toUser != null && fromUser != null)
        //    {
        //        string currentDateTime = DateTime.Now.ToString();
        //        string userImage = string.Empty;
        //        Clients.Client(toUser.ConnectionId).sendIndividualMessage(fromUser.ConnectionId, fromUser.UserName, message, userImage, currentDateTime);
        //        Clients.Client(fromUser.ConnectionId).sendPrivateMessage(toUser.ConnectionId, fromUser.UserName, message, userImage, currentDateTime);
        //    }
        //}


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Clients = null;
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Individual() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
