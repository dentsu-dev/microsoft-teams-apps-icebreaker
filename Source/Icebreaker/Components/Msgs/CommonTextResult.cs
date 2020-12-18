using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.Msgs
{
    public class CommonTextResult
    {
        public Activity Activity { get; set; }
        public string BotMessage { get; set; } 
    }
}