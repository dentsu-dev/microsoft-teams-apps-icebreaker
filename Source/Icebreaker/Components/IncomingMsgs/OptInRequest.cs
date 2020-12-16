using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.IncomingMsgs
{
    public class OptInRequest : IRequest<Activity>
    {
        public Activity Activity { get; set; }
    }
}