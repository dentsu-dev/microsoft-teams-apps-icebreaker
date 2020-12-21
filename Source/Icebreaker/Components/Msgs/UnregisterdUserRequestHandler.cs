using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Properties;
using MediatR;
using Microsoft.Bot.Connector;

namespace Icebreaker.Components.Msgs
{
    public class UnregisterdUserRequestHandler : IRequestHandler<UnregisterdUserRequest, Activity>
    {
        public async Task<Activity> Handle(UnregisterdUserRequest request, CancellationToken cancellationToken)
        {
            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.UnregisteredUserText
                }.ToAttachment(),
            };
            return reply;
        }
    }
}