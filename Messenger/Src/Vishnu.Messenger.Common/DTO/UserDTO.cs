using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vishnu.Messenger.Common.DTO
{
    public class UserDTO : IValidate
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Error { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                Error = "User name cannot be null or empty.";
                return false;
            }
            if (string.IsNullOrEmpty(Email))
            {
                Error = "Email cannot be null or empty.";
                return false;
            }
            if (string.IsNullOrEmpty(Password))
            {
                Error = "Password cannot be null or empty.";
                return false;
            }
            if (string.IsNullOrEmpty(DisplayName))
            {
                Error = "DisplayName cannot be null or empty.";
                return false;
            }
            return true;
        }
    }
}