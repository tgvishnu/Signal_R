using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Core.Configuration
{
    public class DefaultSettings : ISettings
    {
        public DefaultSettings()
        {
            this.Configure();
        }

        public int CacheSize
        {
            get; set;
        }

        public bool EnableCache
        {
            get; set;
        }

        public string UserManagementWebApiBaseAddress
        {
            get; set;
        }

        public void Configure()
        {
            this.CacheSize = 100;
            this.EnableCache = false;
        }
    }
}
