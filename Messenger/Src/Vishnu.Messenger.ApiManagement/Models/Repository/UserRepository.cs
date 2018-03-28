using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vishnu.Messenger.ApiManagement.Models
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private List<User> _usersCollection = new List<User>();

        public UserRepository()
        {
            this.Populate();
        }

        private void Populate()
        {
            User user = new Models.User();
            user.DisplayName = "Trainer";
            user.EmailId = "trainer@gmail.com";
            user.Password = "trainer";
            user.UserId = Guid.NewGuid().ToString();
            user.UserName = "Trainer";
            _usersCollection.Add(user);
            for(int ii=0; ii<10;ii++)
            {
                User usr = new Models.User();
                usr.DisplayName = "student" + ii;
                usr.EmailId = "student" + ii + "@gmail.com";
                usr.Password = "student" + ii;
                usr.UserId = Guid.NewGuid().ToString();
                usr.UserName = "student_" + ii;
                _usersCollection.Add(usr);
            }
        }

        #region IUserRepository

        public User AddUser(User user)
        {
            var users = _usersCollection.Where(e => e.UserName == user.UserName ||
            e.EmailId == user.EmailId).FirstOrDefault();
            if(users == null)
            {
                user.UserId = Guid.NewGuid().ToString();
                _usersCollection.Add(user);
                return user;
            }

            return null;
        }

        public User DeleteUser(string userName)
        {
            var user = _usersCollection.Where(e => e.UserName == userName).FirstOrDefault();
            if(user == null)
            {
                return null;
            }

            var count = _usersCollection.RemoveAll(e => e.UserName == userName);
            return user;
        }


        public User GetUser(string userName)
        {
            var user = _usersCollection.Where(e => e.UserName == userName).FirstOrDefault();
            if (user == null)
            {
                return null;
            }

            return user;
        }

        public User GetUser(string userName, string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetUsers()
        {
            return _usersCollection.ToList();
        }

        public User UpdateUser(User user)
        {
            var deleteUser = this.DeleteUser(user.UserName);
            if(deleteUser == null)
            {
                return null;
            }

            return this.AddUser(user);
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}