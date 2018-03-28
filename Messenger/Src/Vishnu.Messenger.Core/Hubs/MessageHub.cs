using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Communication;
using Vishnu.Messenger.Core.Services;
using Vishnu.Messenger.Core;
using Vishnu.Messenger.Common.Models;
using System.Diagnostics;
using Vishnu.Messenger.Common;

namespace Vishnu.Messenger.Core.Hubs
{
    public class MessageHub : Hub
    {
        private readonly UserStatusManager _userStatusManager = null;
        private readonly ICommunication _broadcast = null;

        public MessageHub()
        {
            _broadcast = Broadcast.Instance;
            _userStatusManager = UserStatusManager.Instance;
        }

        public async Task<IList<UserDetails>> GetUsers()
        {
            UserService service = new Services.UserService(MessengerConfiguration.UserSpecificSettings.UserManagementWebApiBaseAddress);
            return await service.GetUsers();
        }

        public void Hello(string userName)
        {
            string connectionId = Context.ConnectionId;
            _broadcast.SayHello(connectionId, userName, _userStatusManager.ConnectionIds);
        }

        public void Login(string userName, string emailId)
        {
            var id = Context.ConnectionId;
            if (string.IsNullOrEmpty(userName))
            {
                _broadcast.LoginFailure(id, userName, "UserName cannot be null or empty");
            }

            if (string.IsNullOrEmpty(emailId))
            {
                _broadcast.LoginFailure(id, userName, "emailId cannot be null or empty");
            }

            if (_userStatusManager.Contains(id) == false)
            {
                string logintime = DateTime.Now.ToString();
                UserDetails userDetails = new UserDetails()
                {
                    ConnectionId = id,
                    UserName = userName,
                    LoginTime = logintime,
                    EmailId = emailId,
                    Status = Common.UserStatus.Active
                };
                _userStatusManager.Add(userDetails);
                _broadcast.LoginSuccess(id, userDetails, _userStatusManager.ConnectedUsers);
            }
            else
            {
                _broadcast.LoginFailure(id, userName, "UserName already connected.  Please wait some time.");
            }
        }

        public void ChangeStatus(string userName, UserStatus status)
        {
            var id = Context.ConnectionId;
            UserDetails userDetails = _userStatusManager.Update(id, status);
            if (userDetails != null)
            {
                _broadcast.UpdateStatus(id, userDetails, _userStatusManager.ConnectedUsers);
            }
            else
            {
                _broadcast.Failure(id, userName, "User details not found. Unable to update status to " + status);
            }
        }

        public void LogOut(string userName)
        {
            var userDetails = _userStatusManager.Remove(Context.ConnectionId);
            if(userDetails != null)
            {
                userDetails.Status = Common.UserStatus.LogOut;
                _broadcast.LogOut(Context.ConnectionId, userDetails, _userStatusManager.ConnectedUsers);
            }
        }

        public void NotifyAll(string userName, string message)
        {
            string id = Context.ConnectionId;
            var user = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == userName).FirstOrDefault();
            if (user != null)
            {
                _broadcast.NotifyMessage(id, user, message, _userStatusManager.ConnectionIds);
            }
            else
            {
                _broadcast.Failure(id, userName, "User details not found. Unable to notify");
            }
        }

        public void JoinRequest(string chatRoom, UserDetails fromUser, UserDetails toUser, IList<UserDetails> currentUsers)
        {
            string id = Context.ConnectionId;
            try
            {

                if (string.IsNullOrWhiteSpace(chatRoom))
                {
                    _broadcast.Failure(id, fromUser.UserName, "chatRoom is null");
                    return;
                }

                var frUser = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == fromUser.UserName).FirstOrDefault();
                if (frUser == null)
                {
                    _broadcast.Failure(id, fromUser.UserName, "fromUser is null");
                    return;
                }

                var dstUser = _userStatusManager.ConnectedUsers.Where(e => e.UserName == toUser.UserName).FirstOrDefault();
                if (dstUser == null)
                {
                    _broadcast.Failure(id, toUser.UserName, "toUser is null");
                    return;
                }

                _broadcast.JoinRequest(chatRoom, frUser, dstUser, currentUsers);
            }
            catch (Exception ex)
            {
                _broadcast.Failure(id, fromUser.UserName, "Exception occured while sending joining request : " + ex.Message);
            }
        }

        public void NewUserJoinChatRoom(string chatRoom, UserDetails fromUser, UserDetails toUser)
        {
            string id = Context.ConnectionId;
            try
            {

                if (string.IsNullOrWhiteSpace(chatRoom))
                {
                    _broadcast.Failure(id, fromUser.UserName, "chatRoom is null");
                    return;
                }

                var frUser = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == fromUser.UserName).FirstOrDefault();
                if (frUser == null)
                {
                    _broadcast.Failure(id, fromUser.UserName, "fromUser is null");
                    return;
                }

                var dstUser = _userStatusManager.ConnectedUsers.Where(e => e.UserName == toUser.UserName).FirstOrDefault();
                if (dstUser == null)
                {
                    _broadcast.Failure(id, toUser.UserName, "toUser is null");
                    return;
                }

                Clients.Group(chatRoom).onNewUserJoinChatRoom(chatRoom, fromUser, toUser);
            }
            catch (Exception ex)
            {
                _broadcast.Failure(id, fromUser.UserName, "Exception occured while sending joining request : " + ex.Message);
            }
        }

        public async Task<bool> JoinRoom(string chatRoom, UserDetails userDetails)
        {
            string id = Context.ConnectionId;
            bool result = false;
            try
            {

                if (string.IsNullOrWhiteSpace(chatRoom))
                {
                    _broadcast.Failure(id, userDetails.UserName, "chatRoom is null");
                    return result;
                }

                var frUser = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == userDetails.UserName).FirstOrDefault();
                if (frUser == null)
                {
                    _broadcast.Failure(id, userDetails.UserName, "userDetails is null");
                    return result;
                }

                await Groups.Add(userDetails.ConnectionId, chatRoom);
                result = true;
            }
            catch(Exception ex)
            {
                _broadcast.Failure(id, userDetails.UserName, "Exception occured while joining : " + ex.Message);
            }

            return result;
        }

        public async Task<bool> LeaveRoom(string chatRoom, UserDetails userDetails)
        {
            string id = Context.ConnectionId;
            bool result = false;
            try
            {
                if (string.IsNullOrWhiteSpace(chatRoom))
                {
                    _broadcast.Failure(id, userDetails.UserName, "chatRoom is null");
                    return result;
                }

                var frUser = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == userDetails.UserName).FirstOrDefault();
                if (frUser == null)
                {
                    _broadcast.Failure(id, userDetails.UserName, "userDetails is null");
                    return result;
                }

                await Groups.Remove(userDetails.ConnectionId, chatRoom);
                Clients.OthersInGroup(chatRoom).onLeaveRoom(chatRoom, userDetails);
                result = true;
            }
            catch (Exception ex)
            {
                _broadcast.Failure(id, userDetails.UserName, "Exception occured while Leaving : " + ex.Message);
            }
            return result;
        }

        public async Task<bool> SendGroupMessage(string chatRoom, Message message)
        {
            string id = Context.ConnectionId;
            bool result = false;
            try
            {
                if (string.IsNullOrWhiteSpace(chatRoom))
                {
                    _broadcast.Failure(id, message.From.UserName, "chatRoom is null");
                    return result;
                }

                var frUser = _userStatusManager.ConnectedUsers.Where(e => e.ConnectionId == id && e.UserName == message.From.UserName).FirstOrDefault();
                if (frUser == null)
                {
                    _broadcast.Failure(id, message.From.UserName, "userDetails not found");
                    return result;
                }

                await Clients.Group(chatRoom).onGroupChatMessage(chatRoom, message);
                result = true;
            }
            catch (Exception ex)
            {
                _broadcast.Failure(id, message.From.UserName, "Exception occured while Leaving : " + ex.Message);
            }
            return result;
        }

        public override Task OnConnected()
        {
            Debug.WriteLine("OnConnected: " + Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Debug.WriteLine("OnDisconnected: " + Context.ConnectionId);
            var userDetails = _userStatusManager.Remove(Context.ConnectionId);
            if(userDetails != null)
            {
                userDetails.Status = UserStatus.LogOut;
                _broadcast.LogOut(userDetails.ConnectionId, userDetails, _userStatusManager.ConnectedUsers);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Debug.WriteLine("OnReconnected: " + Context.ConnectionId);
            return base.OnReconnected();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            try
            {
                if (_userStatusManager != null)
                {
                    _userStatusManager.Dispose();
                }

                if(_broadcast != null)
                {
                    _broadcast.Dispose();
                }
            }
            catch
            {

            }
            finally
            {
            }
        }
    }
}
