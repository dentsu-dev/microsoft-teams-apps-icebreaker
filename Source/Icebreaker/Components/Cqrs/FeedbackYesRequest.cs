using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.Cqrs
{
    public class FeedbackYesRequest : IRequest<Activity>
    {
        public Activity Activity { get; set; }
    }
}