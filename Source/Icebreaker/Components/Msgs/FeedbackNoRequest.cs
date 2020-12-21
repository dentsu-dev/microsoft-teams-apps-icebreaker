using Icebreaker.Db.Entities;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedbackNoRequest : IRequest<Activity>
    {
        public Activity Activity { get; set; }

        public UserMatchInfo UserMatch { get; set; }

        public BotLastMessageInfo BotLastMessage { get; set; }

    }
}