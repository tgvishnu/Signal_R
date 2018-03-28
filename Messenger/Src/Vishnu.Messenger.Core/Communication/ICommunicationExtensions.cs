using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Common.Models;
using Vishnu.Messenger.Core.Communication;

namespace Vishnu.Messenger.Core
{
    public static class ICommunicationExtensions
    {
        public static void BroadcastMessage(this ICommunication communication, Common.Models.UserDetails fromUser, string message, string dateTime)
        {
            if(communication != null)
            {
                if(communication.Clients != null)
                {
                    communication.Clients.All.broadcastMessage(fromUser, message, dateTime);
                }
            }
        }

        public static void SayHello(this ICommunication communication, string connectionId, string userName, IList<string> connectionIds)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    communication.Clients.Client(connectionId).onHello(connectionId, userName, "Hi " + userName);
                }
            }
        }

        public static void LoginSuccess(this ICommunication communication, string connectionId, UserDetails userDetails, IList<UserDetails> connectedUsers)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    //// send message to caller
                    communication.Clients.Client(connectionId).onLoginSuccess(connectionId, userDetails, connectedUsers);
                    //// send statuc change message to other
                    var connectionIds = connectedUsers.Where(e => e.ConnectionId != connectionId).Select(e => e.ConnectionId).ToList();
                    if (connectionIds != null && connectionId.Count() > 0)
                    {
                        communication.Clients.Clients(connectionIds).onUserStatusChange(connectionId, userDetails);
                    }
                }
            }
        }

        public static void UpdateStatus(this ICommunication communication, string connectionId, UserDetails userDetails, IList<UserDetails> connectedUsers)
        {
            if(communication != null)
            {
                if(communication.Clients != null)
                {
                    var connectionIds = connectedUsers.Select(e => e.ConnectionId).ToList();
                    if (connectionIds != null && connectionId.Count() > 0)
                    {
                        communication.Clients.Clients(connectionIds).onUserStatusChange(connectionId, userDetails);
                    }
                }
            }
        }

        public static void Failure(this ICommunication communication, string connectionId, string userName, string reason)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    communication.Clients.Client(connectionId).onFailure(connectionId, userName, reason);
                }
            }
        }

        public static void LoginFailure(this ICommunication communication, string connectionId, string userName, string reason)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    communication.Clients.Client(connectionId).onLoginFailure(connectionId, userName, reason);
                }
            }
        }

        public static void LogOut(this ICommunication communication, string connectionId, UserDetails userDetails, IList<UserDetails> connectedUsers, bool notifyAll = true)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    //// send logout to caller
                    communication.Clients.Client(connectionId).onLogOutSuccess(connectionId, userDetails);
                    //// send status change event to other users
                    var connectionIds = connectedUsers.Where(e => e.ConnectionId != connectionId).Select(e => e.ConnectionId).ToList();
                    if (connectionIds != null && connectionId.Count() > 0)
                    {
                        communication.Clients.Clients(connectionIds).onUserStatusChange(connectionId, userDetails);
                    }
                }
            }
        }

        private static void SendIndividualMessage(this ICommunication communication, Common.Models.UserDetails toUser, Common.Models.UserDetails fromUser, string message, string userImage, string dateTime)
        {
            if (communication != null)
            {
                if (toUser != null && fromUser != null)
                {
                    communication.Clients.Client(toUser.ConnectionId).sendIndividualMessage(fromUser.ConnectionId, fromUser.UserName, message, userImage, dateTime);
                    communication.Clients.Client(fromUser.ConnectionId).sendIndividualMessage(toUser.ConnectionId, fromUser.UserName, message, userImage, dateTime);
                }
            }
        }

        public static void NotifyMessage(this ICommunication communication, string connectionId, Common.Models.UserDetails fromUser, string message, IList<string> connectionIds)
        {
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                    if (connectionIds != null && connectionIds.Count() > 0)
                    {
                        communication.Clients.Clients(connectionIds).onNotification(connectionId, fromUser, message);
                    }
                }
            }
        }

        public static bool JoinRequest(this ICommunication communication, string chatRoom, UserDetails fromUser, UserDetails toUser, IList<UserDetails> currentUsers)
        {
            bool result = false;
            if (communication != null)
            {
                if (communication.Clients != null)
                {
                     communication.Clients.Client(toUser.ConnectionId).onJoinRequest(chatRoom, fromUser, toUser, currentUsers);
                }
            }

            return result;
        }


    }
}
