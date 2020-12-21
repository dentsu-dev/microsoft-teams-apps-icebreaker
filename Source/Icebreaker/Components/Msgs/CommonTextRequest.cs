using Icebreaker.Components.Msgs;
using Icebreaker.Db.Entities;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.IncomingMsgs
{
    public class CommonTextRequest : IRequest<CommonTextResult>
    {
        public Activity Activity { get; set; }

        public UserMatchInfo UserMatch { get; set; }

        public BotLastMessageInfo BotLastMessage { get; set; }
    }
}