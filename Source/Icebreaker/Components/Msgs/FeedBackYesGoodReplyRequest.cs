using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Icebreaker.Db.Entities;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedBackYesGoodReplyRequest : IRequest<Activity>
    {
        public Activity Activity { get; set; }

        public UserMatchInfo UserMatch { get; set; }

        public BotLastMessageInfo BotLastMessage { get; set; }

    }
}