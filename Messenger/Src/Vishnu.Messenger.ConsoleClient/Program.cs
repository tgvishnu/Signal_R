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
using Vishnu.Messenger.Common;
using Vishnu.Messenger.Common.DTO;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.ConsoleClient
{
    public enum Direction
    {
        In,
        Out
    }
    class Program
    {
        static IHubProxy _proxy = null;
        static string _serverAddress = null;
        static HubConnection _hubConnection = null;
        static string _hubName = null;
        static string _userManagementWebApiBaseAddress = null;
        static UserDetails _userDetails = null;
        static List<UserDetails> chatUsers = new List<UserDetails>();

        static void PrintOptions()
        {
            Print("Select option");
            Print("***********************************\n");
            Print("\t1. Initialize");
            Print("\t2. Deinitialize");
            Print("\t3. Display user details");
            Print("\t100. Register");
            Print("\t101. Get user details");
            Print("\t102. Get all users");
            Print("\t103. Delete user");
            Print("\t200. Send Hello message");
            Print("\t201. Login");
            Print("\t202. Logout");
            Print("\t203. Send notifcation message");
            Print("\t204. ChangeStatus");
            Print("0. Exit");
            Print("\n***********************************");
        }

        static void PrintUserDetails()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Number of User : " + chatUsers.Count);
            foreach(var item in chatUsers)
            {
                sb.AppendLine("\t " + item.ToString());
            }

            Print(sb.ToString());
        }

        private static void _hubConnection_Reconnected()
        {
            Print(string.Format("Reconnected:{0} State:{1}", ConnectionDetails.ConnectionId, _hubConnection.State));
        }

        private static void _hubConnection_Reconnecting()
        {
            Print(string.Format("Reconnecting:{0} State:{1}", ConnectionDetails.ConnectionId, _hubConnection.State));
        }

        private static void _hubConnection_ConnectionSlow()
        {
            Print(string.Format("ConnectionSlow:{0} State:{1}", ConnectionDetails.ConnectionId, _hubConnection.State));
        }

        private static void _hubConnection_Closed()
        {
            Print(string.Format("Closed:{0} State:{1}", ConnectionDetails.ConnectionId, _hubConnection.State));
            ConnectionDetails.ConnectionId = string.Empty;
        }

        static  void Main(string[] args)
        {
            _serverAddress = ConfigurationManager.AppSettings["MessageerServerAddress"];
            _hubName = ConfigurationManager.AppSettings["MessengerHubName"];
            _userManagementWebApiBaseAddress = ConfigurationManager.AppSettings["UserManagementWebApiBaseAddress"];
            string options = string.Empty;
            do
            {
                PrintOptions();
                options = System.Console.ReadLine();
                switch(options)
                {
                    case "1":
                        {
                            Initialize();
                        }
                        break;
                    case "2":
                        {
                            DeInitialize();
                        }
                        break;
                    case "3":
                        DisplayUserDetails();
                        break;
                    case "100":
                        {
                            var task = Task.Factory.StartNew(async () =>
                            {
                                await Register();
                            });
                            task.Wait();
                        }
                        break;
                   
                    case "101":
                        {
                            var task = Task.Factory.StartNew(async () =>
                            {
                                await GetUserDetails();
                            });
                            task.Wait();
                        }
                        break;
                    case "102":
                        {
                            var task = Task.Factory.StartNew(async () =>
                            {
                                await GetAllDetails();
                            });
                            task.Wait();
                        }
                        break;
                    case "103":
                        {
                            var task = Task.Factory.StartNew(async () =>
                            {
                                await DeleteDetails();
                            });
                            task.Wait();
                        }
                        break;
                    case "200":
                        {
                            if(CheckProxy())
                            {
                                Print("Saying Hello to server by " + ConnectionDetails.ConnectionId);
                                _proxy.Invoke<string>("hello" , ConnectionDetails.ConnectionId);
                            }
                        }
                        break;
                    case "201":
                        {
                            Login();
                        }
                        break;
                    case "202":
                        {
                            LogOut();
                        }
                        break;
                    case "203":
                        {
                            Notification();
                        }
                        break;
                    case "204":
                        {
                            ChangeStatus();
                        }
                        break;
                    case "0":
                        options = "n";
                        break;
                }
            } while (options != "n");

            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static bool CheckProxy()
        {
            bool result = false;
            if(_proxy != null)
            {
                if(_hubConnection.State == ConnectionState.Connected)
                {
                    result = true;
                }
            }

            if(result == false)
            {
                Print("Proxy is not connected state.");
            }

            return result;
        }

        static void Initialize()
        {
            DeInitialize();
            Print(string.Format("Initiazling Hub: {0} address: {1} ", _hubName, _serverAddress));
            _hubConnection = new HubConnection(_serverAddress);
            _hubConnection.Closed += _hubConnection_Closed;
            _hubConnection.ConnectionSlow += _hubConnection_ConnectionSlow;
            _hubConnection.Reconnecting += _hubConnection_Reconnecting;
            _hubConnection.Reconnected += _hubConnection_Reconnected;
            _proxy = _hubConnection.CreateHubProxy(_hubName);
            _proxy.On<string, string, string>("onHello", (connectionId, userName, msg) => OnHelloReceived(connectionId, userName, msg));
            _proxy.On<string, UserDetails, IList<UserDetails>>("onLoginSuccess", (connectionId, userDetails, connectedUsers) => OnLoginSuccessReceived(connectionId, userDetails, connectedUsers));
            _proxy.On<string, string, string>("onLoginFailure", (connectionId, userName, reason) => OnLoginFailureReceived(connectionId, userName, reason));
            _proxy.On<string, UserDetails>("onUserStatusChange", (connectionId, userDetails) => OnUserStatusChangeReceived(connectionId, userDetails));
            _proxy.On<string, string, string>("onFailure", (connectionId, userName, failureMessage) => OnFailureReceived(connectionId, userName, failureMessage));
            _proxy.On<string, UserDetails>("onLogOutSuccess", (connectionId, userDetails) => OnLogOutSuccessReceived(connectionId, userDetails));
            _proxy.On<string, UserDetails, string>("onNotification", (connectionId, userDetails, message) => OnNotificationReceived(connectionId, userDetails, message)); 
            _hubConnection.Start().Wait();
            ConnectionDetails.ConnectionId = _hubConnection.ConnectionId;
            //_userDetails.ConnectionId = _hubConnection.ConnectionId;
            Print(string.Format("Initialized on {0} ConnectionId={1}", _serverAddress, ConnectionDetails.ConnectionId));
        }

        static void DeInitialize()
        {
            Print("DeInitializing...");
            try
            {
                chatUsers.Clear();
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
            }
            Print("DeInitialized.");
        }

        static async Task Register()
        {
            Print("Enter user name");
            var userName = Get();
            Print("Enter email id");
            var emailId = Get();
            Print("Enter password");
            var password = Get();
            Print("Confirm Password");
            var confirmPassword = Get();
            Print("Enter display name");
            var displayName = Get();
            UserDTO userdata = new UserDTO();
            userdata.UserName = userName;
            userdata.Email = emailId;
            userdata.Password = password;
            userdata.DisplayName = displayName;
            bool isValid = false;
            if(userdata.Validate())
            {
                if (password == confirmPassword)
                {
                    isValid = true;
                }
                else
                {
                    userdata.Error = "Password and confirm mismatch";
                }
            }

            if(isValid == false)
            {
                Print("Invalid User details. " + userdata.Error);
                return;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.PostAsJsonAsync("users", userdata);
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("User registered successfuly");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if(!string.IsNullOrEmpty(jsonString))
                    {
                        var userData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                        _userDetails = new UserDetails()
                        {
                            ConnectionId = string.Empty,
                            EmailId = userData.Email,
                            UserName = userData.UserName
                        };
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else if(responseTask.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Server response : Bad Request " + jsonString);
                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return;
        }

        static async Task GetUserDetails()
        {
            Print("Enter user name");
            var name = Get();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.GetAsync("users?name=" + name);
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("User registered successfuly");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var userData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                        _userDetails = new UserDetails()
                        {
                            ConnectionId = string.Empty,
                            EmailId = userData.Email,
                            UserName = userData.UserName
                        };

                        Print(_userDetails.ToString());
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else if (responseTask.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Print("Server response : User details not found " + name);
                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return;
        }

        static async Task GetAllDetails()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.GetAsync("users");
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("User details got successfully");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var userData = JsonConvert.DeserializeObject<IList<UserDTO>>(jsonString);
                        foreach (var user in userData)
                        {
                            Print(user.DisplayName + " " + user.Email + " " + user.Password + " " + user.UserId + " " + user.UserName);
                        }
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else if (responseTask.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Print("Server response : User details not found ");
                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return;
        }

        static async Task DeleteDetails()
        {
            Print("Enter user name");
            var name = Get();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_userManagementWebApiBaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage responseTask = await client.DeleteAsync("users?name=" + name);
                if (responseTask.IsSuccessStatusCode)
                {
                    Print("User deleted response received");
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonString))
                    {
                        var userData = JsonConvert.DeserializeObject<UserDTO>(jsonString);
                        _userDetails = new UserDetails()
                        {
                            ConnectionId = string.Empty,
                            EmailId = userData.Email,
                            UserName = userData.UserName
                        };

                        Print("Deleted successfully : " +_userDetails.ToString());
                    }
                    else
                    {
                        Print("Unable to convert user details");
                    }


                }
                else if (responseTask.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Print("Server response : User details not found " + name);
                }
                else
                {
                    var jsonString = await responseTask.Content.ReadAsStringAsync();
                    Print("Internal error : " + jsonString);
                }
            }

            return;
        }

        static void DisplayUserDetails()
        {
            if(_userDetails == null)
            {
                Print("User details are empty.  Get User details");
                return;
            }

            Print(_userDetails.ToString());
        }

        static void Login()
        {
            if(_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            PrintMessage(Direction.Out, "[S]Login ", string.Format("UserName: {0} EmailId:{1}", _userDetails.UserName, _userDetails.EmailId));
            _proxy.Invoke<string>("Login", _userDetails.UserName, _userDetails.EmailId);
        }

        static void LogOut()
        {
            if (_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            if (_userDetails == null)
            {
                Print("User details are null. Get user details");
                return;
            }

            PrintMessage(Direction.Out, "[S]LogOut ", _userDetails.UserName);
            _proxy.Invoke<string>("LogOut", _userDetails.UserName);
        }

        static void Notification()
        {
            if (_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            if (_userDetails == null)
            {
                Print("User details are null. Get user details");
                return;
            }

            Print("Enter notification message");
            string msg = Get();

            PrintMessage(Direction.Out, "[S]NotifyAll", string.Format("UserName: {0} Msg:{1}", _userDetails.UserName, msg));
            _proxy.Invoke<string>("NotifyAll", _userDetails.UserName, msg);
        }

        static void ChangeStatus()
        {
            if (_hubConnection == null)
            {
                Print("Proxy is null. Initiale Proxy");
                return;
            }

            if (_userDetails == null)
            {
                Print("User details are null. Get user details");
                return;
            }

            Print("Enter number to change status [Active=>1] [InActive=>2] [DonotDisturb=>3] [Busy=>4]");
            UserStatus status = _userDetails.Status;
            switch(Get())
            {
                case "1":
                    status = UserStatus.Active;
                    break;
                case "2":
                    status = UserStatus.InActive;
                    break;
                case "3":
                    status = UserStatus.DoNotDisturb;
                    break;
                case "4":
                    status = UserStatus.Busy;
                    break;
            }

            PrintMessage(Direction.Out, "[S]ChangeStatus", string.Format("UserName: {0} Status:{1}", _userDetails.UserName, status));
            _proxy.Invoke<string>("ChangeStatus", _userDetails.UserName, status);
        }

        static string Get()
        {
            return Console.ReadLine();
        }

        static void Print(string message)
        {
            System.Console.WriteLine(message);
        }

        static void PrintMessage(Direction direction, string method, string message)
        {
            Print(string.Format("{0} Method:{1} Message:{2}", (direction == Direction.In ? "<<" : ">>"), method, message));
        }

        static void OnHelloReceived(string connectionId, string userName, string message)
        {
            PrintMessage(Direction.In, "onHello ", string.Format("connectionId:{0} User:{1} Message:{2}", connectionId, userName, message));
        }

        static void OnLoginSuccessReceived(string connectionId, UserDetails userDetails, IList<UserDetails> connectedUsers)
        {
            PrintMessage(Direction.In, "OnLoginSuccess ", string.Format("connectionId:{0} UserName:{1}", connectionId, userDetails.UserName));
            chatUsers.Clear();
            _userDetails = userDetails;
            chatUsers.AddRange(connectedUsers);
            chatUsers.RemoveAll(e => e.ConnectionId == _hubConnection.ConnectionId);
            PrintUserDetails();
        }

        static void OnLoginFailureReceived(string connectionId, string userName, string reason)
        {
            PrintMessage(Direction.In, "OnLoginFailure ", string.Format("connectionId:{0} UserName:{1} Reason:{2}", connectionId, userName, reason));
        }

        static void OnUserStatusChangeReceived(string connectionId, UserDetails userDetails)
        {
            PrintMessage(Direction.In, "onUserStatusChange ", string.Format("ConnectionId: {0}, UserDetails => {1}", connectionId, userDetails.ToString()));
            int count = chatUsers.RemoveAll(e => e.UserName == userDetails.UserName);
            if (count > 0)
            {
                Print("User details removed from list : " + userDetails.UserName);
            }

            chatUsers.Add(userDetails);
            chatUsers.RemoveAll(e => e.UserName == _userDetails.UserName);
            PrintUserDetails();
        }

        static void OnFailureReceived(string connectionId, string userName, string failureMessage)
        {
            PrintMessage(Direction.In, "OnFailure ", string.Format("connectionId:{0} UserName:{1} failureMessage:{2}", connectionId, userName, failureMessage));
        }

        static void OnLogOutSuccessReceived(string connectionId, UserDetails userDetails)
        {
            PrintMessage(Direction.In, "OnLogOutSuccess ", string.Format("connectionId: {0}, userName: {1}", connectionId, userDetails.UserName));
            DeInitialize();
        }

        
        static void OnNotificationReceived(string connectionId, UserDetails userDetails, string message)
        {
            PrintMessage(Direction.In, "OnNotification ", string.Format("ConnectionId: {0} userName: {1} message: {2}", connectionId, userDetails.UserName, message));
        }
    }
}
