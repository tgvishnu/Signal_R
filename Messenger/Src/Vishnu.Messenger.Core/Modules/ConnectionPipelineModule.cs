using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Modules
{
    public class ConnectionPipelineModule : HubPipelineModule
    {
        protected override void OnAfterConnect(IHub hub)
        {
            Debug.WriteLine("=> OnAfterConnect " + hub.Context.ConnectionId);
            base.OnAfterConnect(hub);
        }

        protected override bool OnBeforeConnect(IHub hub)
        {
            Debug.WriteLine("=> OnBeforeConnect " + hub.Context.ConnectionId);
            return base.OnBeforeConnect(hub);
        }

        protected override void OnAfterDisconnect(IHub hub, bool stopCalled)
        {
            Debug.WriteLine("=> OnAfterDisconnect " + hub.Context.ConnectionId);
            base.OnAfterDisconnect(hub, stopCalled);
        }

        protected override bool OnBeforeDisconnect(IHub hub, bool stopCalled)
        {
            Debug.WriteLine("=> OnBeforeDisconnect " + hub.Context.ConnectionId);
            return base.OnBeforeDisconnect(hub, stopCalled);
        }

        protected override void OnAfterReconnect(IHub hub)
        {
            base.OnAfterReconnect(hub);
        }

        protected override bool OnBeforeReconnect(IHub hub)
        {
            return base.OnBeforeReconnect(hub);
        }
    }
}
