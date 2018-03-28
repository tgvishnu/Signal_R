using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.Test.UiClient
{
    public static class UserCache
    {
        public static UserDetails CurrentUser { get; set; }
        public static List<UserDetails> ActualUsers = new List<UserDetails>();
    }
}
