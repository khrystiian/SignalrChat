using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalrChat.Models
{
    public class Message
    {
        public string FromUsername { get; set; }
        public string ToUsername { get; set; }
        public string Msg { get; set; }
        public string DateTime { get; set; }
    }
}