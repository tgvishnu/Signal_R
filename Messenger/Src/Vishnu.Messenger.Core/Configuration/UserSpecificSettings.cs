using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Configuration
{
    public class UserSpecificSettings : IUserSpecificSettings
    {
        public string UserManagementWebApiBaseAddress
        {
            get; set;
        }
    }
}
