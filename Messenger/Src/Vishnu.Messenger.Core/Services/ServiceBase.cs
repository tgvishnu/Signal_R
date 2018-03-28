using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Services
{
    public abstract class ServiceBase : IDisposable
    {
        protected Uri mBaseAddress = null;
        public virtual void Dispose()
        {
        }
    }
}
