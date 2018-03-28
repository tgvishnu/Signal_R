using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Communication;
using Vishnu.Messenger.Core.Configuration;

namespace Vishnu.Messenger.Core
{
    public static class MessengerConfiguration
    {
        public static void Configure(ISettings settings, ICache cache, IUserSpecificSettings userSpecificSettings)
        {
            Settings = settings;
            Cache = cache;
            UserSpecificSettings = UserSpecificSettings;
        }

        public static void Configure(IUserSpecificSettings userSpecificSettings)
        {
            DefaultConfiguration();
            UserSpecificSettings = UserSpecificSettings;
        }

        public static void DefaultConfiguration()
        {
            Settings = new DefaultSettings();
            Cache = new DefaultCache(Settings);
        }


        public static ISettings Settings
        {
            get; set;
        }

        public static ICache Cache
        {
            get; set;
        }

        public static IUserSpecificSettings UserSpecificSettings
        {
            get; set;
        }
    }
}
