using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Communication;
using Vishnu.Messenger.Core.Configuration;

namespace Vishnu.Messenger.Core
{
    public interface IConfiguration
    {
        ISettings Settings { get; set; }
        ICache Cache { get; set; }
    }
}
