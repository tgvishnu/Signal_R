using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Common;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.Core.Communication
{
    public class UserStatusManager : IDisposable
    {
        private readonly List<UserDetails> _connectedUsers = new List<UserDetails>();
        //private readonly static Lazy<UserStatusManager> _instance = new Lazy<UserStatusManager>(() => new UserStatusManager());
        private static UserStatusManager _instance = null;
        private static object _locker = new object();
        private static object _instanceLocker = new object();

        public static UserStatusManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_instanceLocker)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserStatusManager();
                        }
                    }
                }

                return _instance;
            }
        }

        private UserStatusManager()
        {
            Debug.WriteLine("UserStatusManager constructor");
        }

        public IList<UserDetails> ConnectedUsers
        {
            get
            {
                return _connectedUsers;
            }
        }

        public IList<string> ConnectionIds
        {
            get
            {
                lock(_locker)
                {
                    return _connectedUsers.Select(e => e.ConnectionId).ToList();
                }
            }
        }

        public IList<string> FilteredConnectionId(IList<string> users)
        {
            List<string> result = new List<string>();
            foreach(var user in users)
            {
                foreach(var connectedUser in _connectedUsers)
                {
                    if(connectedUser.UserName == user)
                    {
                        result.Add(connectedUser.ConnectionId);
                        break;
                    }
                }
            }

            return result;
        }

        public bool Contains(string connectionId)
        {
            bool result = true;
            lock (_locker)
            {
                if (_connectedUsers.Count(e => e.ConnectionId == connectionId) == 0)
                {
                    result = false;
                }
            }

            return result;
        }

        public void Add(UserDetails user)
        {
            lock(_locker)
            {
                _connectedUsers.Add(user);
            }
        }

        public UserDetails Update(string connectionId, UserStatus status)
        {
            UserDetails userDetails = null;
            lock (_locker)
            {
                foreach (var item in _connectedUsers)
                {
                    if (item.ConnectionId == connectionId)
                    {
                        item.Status = status;
                        userDetails = item;
                        break;
                    }
                }
            }

            return userDetails;
        }

        public UserDetails Remove(string connectionId)
        {
            UserDetails userDetails = null;
            lock (_locker)
            {
                List<UserDetails> users = _connectedUsers.Where(e => e.ConnectionId == connectionId).Select(e => e).ToList();
                int count = _connectedUsers.RemoveAll(e => e.ConnectionId == connectionId);
                if(count > 0)
                {
                    userDetails = users.FirstOrDefault();
                }
            }

            return userDetails;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this._connectedUsers.Clear();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserStatusManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
