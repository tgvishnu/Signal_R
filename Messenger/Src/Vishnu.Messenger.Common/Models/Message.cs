using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.Common.Models
{
    public class Message
    {
        public UserDetails From { get; set; }
        public string UserMessage { get; set; }
        public string MessageTime { get; set; }
    }
}
