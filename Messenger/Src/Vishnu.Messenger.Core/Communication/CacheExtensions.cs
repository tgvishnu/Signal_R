using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Core.Communication;

namespace Vishnu.Messenger.Core
{
    public static class CacheExtensions
    {
        public static Common.Models.Message GetLatest(this ICache cache)
        {
            Common.Models.Message msg= null;
            if(cache != null)
            {
                var collection = cache.Get;
                if(collection != null && collection.Count() > 0)
                {
                    msg = collection.Last();
                }
            }

            return msg;
        }

        public static IEnumerable<Common.Models.Message> Get(this ICache cache, int maxMessages)
        {
            List<Common.Models.Message> result = new List<Common.Models.Message>();
            if(cache != null)
            {
                if(maxMessages >0)
                {
                    var collection = cache.Get.ToList();
                    maxMessages = collection.Count();
                    for(int ii=0; ii< maxMessages; ii++)
                    {
                        result.Add(collection[ii]);
                    }
                }
            }

            return result;
        }
    }
}
