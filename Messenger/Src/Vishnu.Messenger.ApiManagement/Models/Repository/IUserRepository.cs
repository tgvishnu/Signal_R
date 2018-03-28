using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.ApiManagement.Models
{
    public interface IUserRepository
    {
        IEnumerable<User> GetUsers();
        User GetUser(string userName);
        User GetUser(string userName, string id);
        User AddUser(User user);
        User UpdateUser(User user);
        User DeleteUser(string userName);
    }
}
