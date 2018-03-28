using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Communication
{
    public interface ICache : IDisposable
    {
        void Add(string userName, string message, string time, string UserImg);
        IEnumerable<Common.Models.Message> Get { get; }
        void Clear();
        int MaxSize { get; }
    }
}
