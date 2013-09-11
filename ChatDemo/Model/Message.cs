using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatDemo.ViewModel
{
    [Serializable]
    public class Message
    {
        public Message( DateTime whenUtc, string user, string message)
        {
            WhenUTC = whenUtc;
            User = user;
            Text = message;
        }

        public DateTime WhenUTC { get; private set; }
        public string User { get; private set;}
        public string Text { get; private set; } 
    }
}
