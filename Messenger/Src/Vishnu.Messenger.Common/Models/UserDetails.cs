using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Common.Models
{
    public class UserDetails
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string LoginTime { get; set; }
        public string EmailId { get; set; }
        public UserStatus Status { get; set; }
        public UserDetails()
        {
            this.Status = UserStatus.LogOut;
        }
        public override string ToString()
        {
            return string.Format("ConnectionID:{0} UserName:{1} LoginTime:{2} EmailId:{3} Status:{4}", this.ConnectionId, this.UserName, this.LoginTime, this.EmailId, this.Status);
        }
    }
}
