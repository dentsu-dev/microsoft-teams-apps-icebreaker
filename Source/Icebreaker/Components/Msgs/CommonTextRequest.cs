using Icebreaker.Components.Msgs;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.IncomingMsgs
{
    public class CommonTextRequest : IRequest<CommonTextResult>
    {
        public Activity Activity { get; set; }
    }
}