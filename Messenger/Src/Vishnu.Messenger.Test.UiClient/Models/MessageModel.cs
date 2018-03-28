using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vishnu.Messenger.Common.Models;

namespace Vishnu.Messenger.Test.UiClient.Models
{
    public class MessageModel
    {
        public bool  IsPublished { get; set; }
        public bool IsSender { get; set; }

        public string Direction
        {
            get
            {
                if(IsSender)
                {
                    return ">> ";
                }
                else
                {
                    return "<< ";
                }
            }
        }

        public string ShortTime
        {
            get
            {
                string result = string.Empty;
                if (MessageDetails != null)
                {
                    try

                    {
                        DateTime dt = DateTime.Parse(MessageDetails.MessageTime);
                        result = string.Format(dt.Hour.ToString() + ":" + dt.Minute.ToString());
                    }
                    catch
                    {
                    }
                }

                return result;
            }
        }
        public Message MessageDetails { get; set; }
        public MessageModel()
        {
        }

        public string Format()
        {
            StringBuilder sb = new StringBuilder();
            if (MessageDetails != null)
            {
                sb.AppendLine(IsSender ? "->> " : "<<- " + MessageDetails.MessageTime + ": " + MessageDetails.From.UserName);
                sb.AppendLine("\t" + MessageDetails.UserMessage);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (MessageDetails != null)
            {
                sb.Append(IsSender == true ? "> " : "< ");
                sb.Append(MessageDetails.MessageTime + " ");
                sb.AppendLine(MessageDetails.From.UserName);
                sb.Append("\t" + MessageDetails.UserMessage);
            }

            return sb.ToString();
        }
    }
}
