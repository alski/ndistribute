using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ChatDemo.ViewModel
{
    [Serializable]
    public class MessageViewModel
    {
        private Message _message;

        public MessageViewModel(Message message, bool isLocal)
        {
            _message = message;
            TextBrush = isLocal ? SystemColors.WindowTextBrush : SystemColors.GrayTextBrush;
        }

        public DateTime WhenUTC { get { return _message.WhenUTC; } }
        public string User { get { return _message.User; } }
        public string Message { get { return _message.Text; } }
        public Brush TextBrush { get; private set; }
    }
}
