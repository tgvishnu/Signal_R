using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Configuration;

namespace Vishnu.Messenger.Core.Communication
{
    public class DefaultCache : ICache
    {
        private static readonly List<Common.Models.Message> _cacheMessage = new List<Common.Models.Message>();
        private static object _locker = new object();
        private readonly ISettings _settings = null;
        public DefaultCache(ISettings settings)
        {
            this._settings = settings;
        }

        public int MaxSize
        {
            get
            {
                if(_settings != null)
                {
                    return _settings.CacheSize;
                }

                return 0;
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                if (_cacheMessage != null)
                {
                    _cacheMessage.Clear();
                }
            }
        }

        public void Add(string userName, string userMessage, string time, string userImage)
        {
            lock (_locker)
            {
                _cacheMessage.Add(new Common.Models.Message()
                {
                    MessageTime = time,
                    UserMessage = userMessage,
                    //UserName = userName,
                });

                if (_cacheMessage.Count > this.MaxSize)
                {
                    _cacheMessage.RemoveAt(0);
                }
            }
        }

        public IEnumerable<Common.Models.Message> Get
        {
            get
            {
                return _cacheMessage.ToList();
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_cacheMessage != null)
                    {
                        _cacheMessage.Clear();
                    }
                }

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
