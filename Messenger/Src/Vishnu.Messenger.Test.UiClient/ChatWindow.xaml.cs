using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Vishnu.Messenger.Common.Models;
using Vishnu.Messenger.Test.UiClient.Models;

namespace Vishnu.Messenger.Test.UiClient
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        List<UserDetails> _chatUsers = new List<UserDetails>();
        IHubProxy _proxy = null;
        public ChatWindow(IHubProxy proxy, string chatContextId, IList<UserDetails> currentUsers)
        {
            InitializeComponent();
            _chatUsers.Clear();
            _proxy = proxy;
            this.ChatContextId = chatContextId;
            _chatUsers.AddRange(currentUsers);
            this.txtCharRoomId.Text = chatContextId;
            this.txtCurrentUser.Text = UserCache.CurrentUser.UserName;
            this.UpdateCollection();
        }

        public string ChatContextId
        {
            get;
            private set;
        }

        public IList<UserDetails> ChatUsers
        {
            get
            {
                return _chatUsers;
            }
        }

        public void AddUser(UserDetails newUser)
        {
            var user = _chatUsers.Where(e => e.UserName == newUser.UserName).Select(e => e).FirstOrDefault();
            if(user == null)
            {
                _chatUsers.Add(newUser);
                this.UpdateCollection();
            }
        }

        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await ChatWindowService.Instance.LeaveChatRoom(this.txtCharRoomId.Text, UserCache.CurrentUser);
        }

        private void UpdateCollection()
        {
            this.lstUsers.Items.Clear();
            foreach(var item in _chatUsers)
            {
                this.lstUsers.Items.Add(item);
            }
        }

        public void ChangeStatus(UserDetails userDetails)
        {
            int count = _chatUsers.RemoveAll(e => e.UserName == userDetails.UserName);
            if (count > 0)
            {
                _chatUsers.Add(userDetails);
                this.UpdateCollection();
            }
        }


        public void RemoveUser(UserDetails userDetails)
        {
            int count = _chatUsers.RemoveAll(e => e.UserName == userDetails.UserName);
            if(count > 0)
            {
                this.UpdateCollection();
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            UsersWindow windows = new UiClient.UsersWindow(ChatContextId, _chatUsers);
            windows.ShowDialog();
        }

        public void MessageReceived(Message msg)
        {
            MessageModel msgModel = new MessageModel();
            if (msg.From.UserName != UserCache.CurrentUser.UserName)
            {
                msgModel.IsSender = false;
                msgModel.MessageDetails = msg;
                this.lstConversation.Items.Add(msgModel);
                this.txtLastMessageReceived.Text = "Last Message Received on " + msgModel.MessageDetails.MessageTime;
                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            var selectedIndex = lstConversation.Items.Count - 1;
            if (selectedIndex < 0)
                return;
            lstConversation.SelectedIndex = selectedIndex;
            lstConversation.UpdateLayout();
            lstConversation.ScrollIntoView(lstConversation.SelectedItem);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void txtSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (txtSend.Text.Trim() != string.Empty)
                {
                    if (_proxy != null)
                    {
                        MessageModel msgModel = new MessageModel();
                        msgModel.IsSender = true;
                        msgModel.MessageDetails = new Message()
                        {
                            From = UserCache.CurrentUser,
                            UserMessage = this.txtSend.Text.Trim(),
                            MessageTime = DateTime.Now.ToString()
                        };

                        this.lstConversation.Items.Add(msgModel);
                        this.txtLastMessageReceived.Text = "Last Message Sent on " + msgModel.MessageDetails.MessageTime;
                        bool result = await _proxy.Invoke<bool>("SendGroupMessage", this.ChatContextId, msgModel.MessageDetails);
                        msgModel.IsPublished = result;
                        txtSend.Text = string.Empty;
                    }
                }
            }

            return;
        }
    }
}
