using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Vishnu.Messenger.Test.UiClient.Models;

namespace Vishnu.Messenger.Test.UiClient.Resources.TemplateSelectors
{
    public class ChatMessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SenderTemplate { get; set; }
        public DataTemplate ReceiverTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return null;
            FrameworkElement frameworkElement = container as FrameworkElement;
            if(frameworkElement != null)
            {
                bool isSender = ((MessageModel)item).IsSender;
                if(isSender)
                {
                    SenderTemplate = frameworkElement.FindResource("senderUserTemplate") as DataTemplate;
                    return SenderTemplate;
                }
                else
                {
                    ReceiverTemplate = frameworkElement.FindResource("receiverUserTemplate") as DataTemplate;
                    return ReceiverTemplate;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
