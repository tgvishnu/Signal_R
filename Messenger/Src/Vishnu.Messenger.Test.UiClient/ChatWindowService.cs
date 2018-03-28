using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.Test.UiClient
{



    public class ChatWindowService
    {
        private static ChatWindowService _instance = null;
        private static object _instanceLocker = new object();
        private IHubProxy _proxy = null;
        private ConcurrentDictionary<string, ChatWindow> _chatWindowCollection = new ConcurrentDictionary<string, ChatWindow>();

        public void SetProxy(IHubProxy proxy)
        {
            _proxy = proxy;
        }

        private ChatWindowService()
        {
        }

        public static ChatWindowService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_instanceLocker)
                    {
                        if(_instance == null)
                        {
                            _instance = new ChatWindowService();
                        }
                    }
                }

                return _instance;
            }
        }

        public async void StartChat(string chatContextId, IHubProxy proxy, UserDetails fromUser, UserDetails withUser, IList<UserDetails> currentUsers)
        {
            //// from originator instantiated chat
            if (chatContextId == null)
            {
                ChatWindow window = null;
                foreach (var kv in _chatWindowCollection)
                {
                    if (kv.Value.ChatUsers.Count == 1)
                    {
                        if (kv.Value.ChatUsers[0].UserName == withUser.UserName)
                        {
                            window = kv.Value;
                            break;
                        }
                    }
                }

                if (window != null)
                {
                    window.Activate();
                }
                else
                {
                    var chatRoom = Guid.NewGuid().ToString() + "[" + fromUser.EmailId + "]";
                    ChatWindow newWindow = new ChatWindow(_proxy, chatRoom, currentUsers);
                    newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    if (_chatWindowCollection.TryAdd(chatRoom, newWindow))
                    {
                        bool result = await proxy.Invoke<bool>("JoinRoom", chatRoom, fromUser);
                        if (result == true)
                        {
                            newWindow.Show();
                            await proxy.Invoke("JoinRequest", chatRoom, fromUser, withUser, currentUsers);
                        }
                        else
                        {
                            ChatWindow removeWindow;
                            if (_chatWindowCollection.TryRemove(chatRoom, out removeWindow) == false)
                            {
                                MessageBox.Show("[O] Unable to remove user");
                            }

                            MessageBox.Show("[O] Unable to Start chat. Try again.");
                        }
                    }
                }
            }
            //// recepient
            else
            {
                ChatWindow newWindow = null;
                if (_chatWindowCollection.ContainsKey(chatContextId))
                {
                    if (_chatWindowCollection.TryGetValue(chatContextId, out newWindow))
                    {
                        newWindow.Activate();
                    }
                    else
                    {
                        MessageBox.Show("Unable to open chat room requested by " + fromUser.UserName);
                    }
                }
                else
                {
                    // This will add new group owner.
                    var window  = new ChatWindow(_proxy, chatContextId, currentUsers);
                    window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    if (_chatWindowCollection.TryAdd(chatContextId, window))
                    {
                        // join current user.
                        bool result = await proxy.Invoke<bool>("JoinRoom", chatContextId, withUser);
                        if (result == true)
                        {
                            window.Show();
                            await _proxy.Invoke("NewUserJoinChatRoom", chatContextId, UserCache.CurrentUser, UserCache.CurrentUser);
                        }
                        else
                        {
                            ChatWindow removeWindow;
                            if (_chatWindowCollection.TryRemove(chatContextId, out removeWindow) == false)
                            {
                                MessageBox.Show("Unable to remove user");
                            }

                            MessageBox.Show("Unable to Start chat. Try again.");
                        }
                    }
                }
            }
        }

        public void NotifyChangeStatus(UserDetails userDetails)
        {
            foreach(var kv in _chatWindowCollection)
            {
                kv.Value.ChangeStatus(userDetails);
            }

        }

        public async Task LeaveChatRoom(string chatContextId, UserDetails userDetails)
        {
            bool result =  await _proxy.Invoke<bool>("LeaveRoom", chatContextId, userDetails);
            if(result == false)
            {
                MessageBox.Show("Unable to leave chat room.  Removed forcefully.");
            }

            ChatWindow window = null;
            if(!this._chatWindowCollection.TryRemove(chatContextId, out window))
            {
                MessageBox.Show("Unable to remove ChatRoom : " + chatContextId);
            }
        }

        public void NotifyLeaveChatRoom(string chatContextId, IHubProxy proxy, UserDetails userDetails)
        {
            ChatWindow window = null;
            if (!this._chatWindowCollection.TryGetValue(chatContextId, out window))
            {
                MessageBox.Show("Unable to get ChatRoom : " + chatContextId);
                return;
            }

            window.RemoveUser(userDetails);
        }

        public async Task JoinNewUserChatRoom(string chatContextId, UserDetails fromUser, UserDetails newUser, IList<UserDetails> currentUsers)
        {
            await _proxy.Invoke("JoinRequest", chatContextId, fromUser, newUser, currentUsers);
        }

        public void NotifyJoinNewUserChatRoom(string chatContextId, IHubProxy proxy, UserDetails userDetails)
        {
            ChatWindow window = null;
            if (!this._chatWindowCollection.TryGetValue(chatContextId, out window))
            {
                MessageBox.Show("Unable to get ChatRoom : " + chatContextId);
                return;
            }

            window.AddUser(userDetails);
        }

        public void NotifyMessageReception(string chatContextId, Message message)
        {
            ChatWindow newWindow = null;
            lock (_chatWindowCollection)
            {
                if (_chatWindowCollection.ContainsKey(chatContextId))
                {
                    if (!_chatWindowCollection.TryGetValue(chatContextId, out newWindow))
                    {
                    }
                }
            }

            if(newWindow != null)
            {
                newWindow.MessageReceived(message);
            }
            else
            {
                MessageBox.Show("Cannot get chat window " + chatContextId);
            }
        }

    }
}
