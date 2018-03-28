using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vishnu.Messenger.Common;
using Vishnu.Messenger.Common.DTO;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.Test.UiClient
{
    public enum Direction
    {
        In,
        Out
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IHubProxy _proxy = null;
        string _serverAddress = null;
        HubConnection _hubConnection = null;
        string _hubName = null;
        string _userManagementWebApiBaseAddress = null;

        public MainWindow()
        {
            InitializeComponent();
            _serverAddress = ConfigurationManager.AppSettings["MessageerServerAddress"];
            _hubName = ConfigurationManager.AppSettings["MessengerHubName"];
            _userManagementWebApiBaseAddress = ConfigurationManager.AppSettings["UserManagementWebApiBaseAddress"];
            Task.Run(async () =>
            {
                await Initialize();
            });
        }

        private void btnHello_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSendNotification_Click(object sender, RoutedEventArgs e)
        {
            if (_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            if (UserCache.CurrentUser == null)
            {
                Print("User details are null. Get user details");
                return;
            }

            Print("Enter notification message");
            string msg = this.txtReceiveNotfication.Text;
            PrintMessage(Direction.Out, "[S]NotifyAll", string.Format("UserName: {0} Msg:{1}", UserCache.CurrentUser.UserName, msg));
             _proxy.Invoke<string>("NotifyAll", UserCache.CurrentUser.UserName, msg);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            this.txtReceiveNotfication.Text = string.Empty;
            this.lstLog.Items.Clear();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtUserName.Text))
            {
                MessageBox.Show("User name cannot be null or empty", "Messenger", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_hubConnection == null)
            {
                Print("Initializing Hub");
                ClearAllFields();
                await Initialize();
            }

            if (string.IsNullOrEmpty(this.txtUserName.Text))
            {
                MessageBox.Show("User name cannot be null or empty","Messenger", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try

            {
                UserCache.CurrentUser = await GetUserDetails(this.txtUserName.Text);
                if (UserCache.CurrentUser != null)
                {
                    UserCache.CurrentUser.ConnectionId = _hubConnection.ConnectionId;
                    this.txtEmail.Text = UserCache.CurrentUser.EmailId;
                    //this.txtConnectionId.Text = UserCache.CurrentUser.ConnectionId;
                    this.txtStatus.Text = UserCache.CurrentUser.Status.ToString();
                    this.UpdateTextUserStatus();
                }
                else
                {
                    MessageBox.Show("Unknown user : " + this.txtUserName.Text, "Messenger", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                var users = await GetUsers();
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        UserCache.ActualUsers.Add(user);
                        this.lstChatUser.Items.Add(user);
                    }
                }


                await _proxy.Invoke<string>("Login", UserCache.CurrentUser.UserName, UserCache.CurrentUser.EmailId);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to login. Error : " + ex.Message, "Messenger", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return;
        }


        private void ClearAllFields()
        {
            if (UserCache.CurrentUser != null)
            {
                this.txtUserName.Text = UserCache.CurrentUser.UserName;
            }
            
            UserCache.CurrentUser = null;
            this.lstChatUser.Items.Clear();
            this.txtEmail.Text = string.Empty;
            //this.txtConnectionId.Text = string.Empty;
            this.txtStatus.Text = string.Empty;
            this.UpdateTextUserStatus();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hubConnection == null)
                {
                    Print("Proxy is null. Initiale Proxy");
                    return;
                }

                if (UserCache.CurrentUser == null)
                {
                    Print("User details are null. Get user details");
                    return;
                }

                PrintMessage(Direction.Out, "[S]LogOut ", UserCache.CurrentUser.UserName);
                _proxy.Invoke<string>("LogOut", UserCache.CurrentUser.UserName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to logOut. Error : " + ex.Message, "Messenger", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task<UserDetails> GetUserDetails(string username)
        {
            UserDetails userDetails = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.GetAsync("users?name=" + username);
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("Got User details");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var userData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                        userDetails = new UserDetails()
                        {
                            ConnectionId = string.Empty,
                            EmailId = userData.Email,
                            UserName = userData.UserName
                        };

                        Print(userDetails.ToString());
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else if (responseTask.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Print("Server response : User details not found " + username);
                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return userDetails;
        }

        private async Task<List<UserDetails>> GetUsers()
        {
            List<UserDetails> users = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.GetAsync("users");
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("Got all users");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var deserializedData = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString);
                        if(deserializedData != null)
                        {
                            users = new List<UserDetails>();
                            foreach (var item in deserializedData)
                            {
                                users.Add(new UserDetails()
                                {
                                    ConnectionId = string.Empty,
                                    EmailId = item.Email,
                                    UserName = item.UserName
                                });
                            }

                            Print("Total Users: " + users.Count.ToString());
                        }
                        else
                        {
                            Print("Deserailize data is null");
                        }
                        
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return users;
        }

        private async Task Initialize()
        {
            await DeInitialize();
            Print(string.Format("Initiazling Hub: {0} address: {1} ", _hubName, _serverAddress));
            _hubConnection = new HubConnection(_serverAddress);
            _hubConnection.Closed += _hubConnection_Closed;
            _hubConnection.ConnectionSlow += _hubConnection_ConnectionSlow;
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Reconnected += _hubConnection_Reconnected;
            _proxy = _hubConnection.CreateHubProxy(_hubName);
            //_proxy.On<string, string, string>("onHello", (connectionId, userName, msg) => OnHelloReceived(connectionId, userName, msg));
            _proxy.On<string, UserDetails, IList<UserDetails>>("onLoginSuccess", (connectionId, userDetails, connectedUsers) => OnLoginSuccessReceived(connectionId, userDetails, connectedUsers));
            _proxy.On<string, string, string>("onLoginFailure", (connectionId, userName, reason) => OnLoginFailureReceived(connectionId, userName, reason));
            _proxy.On<string, UserDetails>("onUserStatusChange", (connectionId, userDetails) => OnUserStatusChangeReceived(connectionId, userDetails));
            _proxy.On<string, string, string>("onFailure", (connectionId, userName, failureMessage) => OnFailureReceived(connectionId, userName, failureMessage));
            _proxy.On<string, UserDetails>("onLogOutSuccess", (connectionId, userDetails) => OnLogOutSuccessReceived(connectionId, userDetails));
            _proxy.On<string, UserDetails, string>("onNotification", (connectionId, userDetails, message) => OnNotificationReceived(connectionId, userDetails, message));
            _proxy.On<string, UserDetails, UserDetails, IList<UserDetails>>("onJoinRequest", (chatRoom, fromUser, toUser, currentUsers) => OnJoinRequestReceived(chatRoom, fromUser, toUser, currentUsers));
            _proxy.On<string, UserDetails>("onLeaveRoom", (chatRoom, userDetails) => OnLeaveRoomReceived(chatRoom, userDetails));
            _proxy.On<string, UserDetails, UserDetails>("onNewUserJoinChatRoom", (chatRoom, fromUser, toUser) => OnNewUserJoinChatRoomReceived(chatRoom, fromUser, toUser));
            _proxy.On<string, Message>("onGroupChatMessage", (chatRoom, message) => OnGroupChatMessageReceived(chatRoom, message));
            await _hubConnection.Start();
            Print(string.Format("Initialized on {0} ConnectionId={1}", _serverAddress, _hubConnection.ConnectionId));
            return;
        }

        private async Task DeInitialize()
        {
            await Task.Factory.StartNew(() => {
            Print("DeInitializing...");
            try
            {
                if (_hubConnection != null)
                {
                    _hubConnection.Stop();
                    _hubConnection.Closed -= _hubConnection_Closed;
                    _hubConnection.ConnectionSlow -= _hubConnection_ConnectionSlow;
                    _hubConnection.Reconnecting -= _hubConnection_Reconnecting;
                    _hubConnection.Reconnected -= _hubConnection_Reconnected;
                    _hubConnection.Dispose();
                }
            }
            catch
            {
            }
            finally
            {
                _hubConnection = null;
                _proxy = null;
                UserCache.CurrentUser = null;
            }

            Print("DeInitialized.");
        });
            
        }

        private  void _hubConnection_Reconnected()
        {
            Print(string.Format("Reconnected:{0} State:{1}", _hubConnection.ConnectionId, _hubConnection.State));
        }

        private  void _hubConnection_Reconnecting()
        {
            Print(string.Format("Reconnecting:{0} State:{1}", _hubConnection.ConnectionId, _hubConnection.State));
        }

        private  void _hubConnection_ConnectionSlow()
        {
            Print(string.Format("ConnectionSlow:{0} State:{1}", _hubConnection.ConnectionId, _hubConnection.State));
        }

        private  void _hubConnection_Closed()
        {
            Print(string.Format("Closed:{0} State:{1}", _hubConnection.ConnectionId, _hubConnection.State));
        }

        private void Print(string message)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.lstLog.Items.Add(message);

            }));
        }

        private void PrintMessage(Direction direction, string method, string message)
        {
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                this.lstLog.Items.Add(string.Format("{0} Method:{1} Message:{2}", (direction == Direction.In ? "<<" : ">>"), method, message));
            }));
        }

        void OnLoginSuccessReceived(string connectionId, UserDetails userDetails, IList<UserDetails> connectedUsers)
        {
            PrintMessage(Direction.In, "OnLoginSuccess ", string.Format("connectionId:{0} UserName:{1}", connectionId, userDetails.UserName));
            UserCache.CurrentUser = userDetails;
            this.UpdateUserStatus(userDetails, connectedUsers);
            ChatWindowService.Instance.SetProxy(_proxy);
        }

        private void UpdateUserStatus(UserDetails sender, IList<UserDetails> connectedUsers)
        {
            foreach (var connectedUser in connectedUsers)
            {
                foreach (var user in UserCache.ActualUsers)
                {
                    if (connectedUser.UserName == user.UserName)
                    {
                        user.Status = connectedUser.Status;
                        user.ConnectionId = connectedUser.ConnectionId;
                        break;
                    }
                }
            }

            UserDetails currentUser = null;
            if (sender.UserName == UserCache.CurrentUser.UserName)
            {
                currentUser = sender;
            }
            //UserDetails currentUser = _ActualUsers.Where(e => e.UserName == _userDetails.UserName).Select(e => e).FirstOrDefault();
            UserCache.ActualUsers.RemoveAll(e => e.UserName == UserCache.CurrentUser.UserName);
            this.Dispatcher.BeginInvoke((Action)(() =>
           {
               if(currentUser != null)
               {
                   UserCache.CurrentUser = currentUser;
                   this.txtStatus.Text = UserCache.CurrentUser.Status.ToString();
                   this.UpdateTextUserStatus();
               }
               this.lstChatUser.Items.Clear();
               foreach (var user in UserCache.ActualUsers)
               {
                   this.lstChatUser.Items.Add(user);
               }

               this.txtUserName.Foreground = new SolidColorBrush(Colors.Black);
               txtUserName.BorderThickness = new Thickness(0, 0, 0, 0);
           }));
        }

        void OnUserStatusChangeReceived(string connectionId, UserDetails userDetails)
        {
            PrintMessage(Direction.In, "onUserStatusChange ", string.Format("ConnectionId: {0}, UserDetails => {1}", connectionId, userDetails.ToString()));
            this.UpdateUserStatus(userDetails, new List<UserDetails>() { userDetails});
            this.Dispatcher.BeginInvoke((Action) (() => {
                ChatWindowService.Instance.NotifyChangeStatus(userDetails);
            }));
        }


        void OnLoginFailureReceived(string connectionId, string userName, string reason)
        {
            PrintMessage(Direction.In, "OnLoginFailure ", string.Format("connectionId:{0} UserName:{1} Reason:{2}", connectionId, userName, reason));
        }
        
        void OnFailureReceived(string connectionId, string userName, string failureReason)
        {
            PrintMessage(Direction.In, "OnFailure ", string.Format("connectionId:{0} UserName:{1} Reason:{2}", connectionId, userName, failureReason));
        }

        void OnLogOutSuccessReceived(string connectionId, UserDetails userDetails)
        {
            PrintMessage(Direction.In, "OnLogOutSuccess ", string.Format("connectionId: {0}, userName: {1}", connectionId, userDetails.UserName));
            this.Dispatcher.BeginInvoke((Action)(() =>
           {
               this.ClearAllFields();
           }));
            Task.Run(async () =>
            {
                await DeInitialize();
            });
        }

         void OnNotificationReceived(string connectionId, UserDetails userDetails, string message)
        {
            PrintMessage(Direction.In, "OnNotification ", string.Format("ConnectionId: {0} userName: {1} message: {2}", connectionId, userDetails.UserName, message));
            if(userDetails.UserName != UserCache.CurrentUser.UserName)
                this.Dispatcher.BeginInvoke((Action)(() =>{
                this.txtReceiveNotfication.Text = string.Format("{0} => {1}", userDetails.UserName, message);
            }));
        }

        void OnJoinRequestReceived(string chatRoom, UserDetails fromUser, UserDetails toUser, IList<UserDetails> currentUsers)
        {
            PrintMessage(Direction.In, "OnJoinRequest ", string.Format("chatRoom: {0} FromUserName: {1} ToUserName:{2}", chatRoom, fromUser.UserName, toUser.UserName));
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ChatWindowService.Instance.StartChat(chatRoom, _proxy, fromUser, toUser, currentUsers);
            }));
        }
        
        void OnNewUserJoinChatRoomReceived(string chatRoom, UserDetails fromUser, UserDetails toUser)
        {
            PrintMessage(Direction.In, "OnNewUserJoinChatRoom ", string.Format("chatRoom: {0} FromUserName: {1} ToUserName:{2}", chatRoom, fromUser.UserName, toUser.UserName));
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ChatWindowService.Instance.NotifyJoinNewUserChatRoom(chatRoom, _proxy, toUser);
            }));
        }

        void OnLeaveRoomReceived(string chatRoom, UserDetails userDetails)
        {
            PrintMessage(Direction.In, "OnLeaveRoom ", string.Format("chatRoom: {0} userDetails: {1}", chatRoom, userDetails.UserName));
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ChatWindowService.Instance.NotifyLeaveChatRoom(chatRoom, _proxy, userDetails);
            }));
        }

        void OnGroupChatMessageReceived(string chatRoom, Message message)
        {
            PrintMessage(Direction.In, "onGroupChatMessage ", string.Format("chatRoom: {0} userDetails: {1}", chatRoom, message.From.UserName));
            this.Dispatcher.BeginInvoke((Action)(() =>
            {
                ChatWindowService.Instance.NotifyMessageReception(chatRoom, message);
            }));
        }

        

        private void lstChatUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lstChatUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (UserDetails)this.lstChatUser.SelectedItem;
            if(selectedItem != null)
            {
                if(selectedItem.Status == UserStatus.LogOut)
                {
                    MessageBox.Show("User is logged out. Cannot chat with him", "Messenger", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                ChatWindowService.Instance.StartChat(null, _proxy, UserCache.CurrentUser, selectedItem, new List<UserDetails>() { UserCache.CurrentUser, selectedItem });
            }
        }

        private void txtUserName_MouseEnter(object sender, MouseEventArgs e)
        {
            txtUserName.BorderThickness = new Thickness(1, 1, 1, 1);
            if (txtUserName.Text == "Enter User Name")
                txtUserName.Text = string.Empty;
        }

        private void txtUserName_MouseLeave(object sender, MouseEventArgs e)
        {
            txtUserName.BorderThickness = new Thickness(0,0,0,0);
            if (txtUserName.Text == "")
                txtUserName.Text = "Enter User Name";
        }

        private async void btnExit_Click(object sender, RoutedEventArgs e)
        {
            await this.DeInitialize();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            if (UserCache.CurrentUser == null)
            {
                Print("User details are null. Get user details");
                return;
            }

            UserStatus status = UserCache.CurrentUser.Status;
            MenuItem item = (MenuItem)sender;
            switch ((string)item.CommandParameter)
            {
                case "Active":
                    status = UserStatus.Active;
                    break;
                case "InActive":
                    status = UserStatus.InActive;
                    break;
                case "DoNotDisturb":
                    status = UserStatus.DoNotDisturb;
                    break;
                case "Busy":
                    status = UserStatus.Busy;
                    break;
            }

            PrintMessage(Direction.Out, "[S]ChangeStatus", string.Format("UserName: {0} Status:{1}", UserCache.CurrentUser.UserName, status));
            _proxy.Invoke<string>("ChangeStatus", UserCache.CurrentUser.UserName, status);
        }

        private void UpdateTextUserStatus()
        {
            if (UserCache.CurrentUser != null)
            {
                switch (UserCache.CurrentUser.Status)
                {
                    case UserStatus.Active:
                        this.elpStatus.Fill = new SolidColorBrush(Colors.YellowGreen);
                        break;
                    case UserStatus.Busy:
                        this.elpStatus.Fill = new SolidColorBrush(Colors.Orange);
                        break;
                    case UserStatus.DoNotDisturb:
                        this.elpStatus.Fill = new SolidColorBrush(Colors.Red);
                        break;
                    case UserStatus.InActive:
                        this.elpStatus.Fill = new SolidColorBrush(Colors.Yellow);
                        break;
                    case UserStatus.LogOut:
                        this.elpStatus.Fill = new SolidColorBrush(Colors.LightGray);
                        break;
                }
            }
            else
            {
                this.elpStatus.Fill = new SolidColorBrush(Colors.LightGray);
            }

        }
    }
}
