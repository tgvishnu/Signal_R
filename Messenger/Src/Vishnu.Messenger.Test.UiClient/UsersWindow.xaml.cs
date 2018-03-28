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

namespace Vishnu.Messenger.Test.UiClient
{
    /// <summary>
    /// Interaction logic for UsersWindow.xaml
    /// </summary>
    public partial class UsersWindow : Window
    {
        private string _chatContextID = string.Empty;
        private IList<UserDetails> _currentUsers = null;
        public UsersWindow(string chatContextID, IList<UserDetails> currentUsers)
        {
            InitializeComponent();
            this.UpdateList();
            _chatContextID = chatContextID;
            txtCharRoomId.Text = chatContextID;
            _currentUsers = currentUsers;
            if(currentUsers != null)
            {
                txtCurrentUserCount.Text = currentUsers.Count.ToString();
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (UserDetails) this.lstChatUser.SelectedItem;
            if(selectedUser != null)
            {
                await ChatWindowService.Instance.JoinNewUserChatRoom(_chatContextID, UserCache.CurrentUser, selectedUser, _currentUsers);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateList()
        {
            foreach(var user in UserCache.ActualUsers)
            {
                this.lstChatUser.Items.Add(user);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
