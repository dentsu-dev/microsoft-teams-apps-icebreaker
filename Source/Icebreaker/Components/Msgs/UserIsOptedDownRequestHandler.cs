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
    public class UserIsOptedDownRequestHandler : IRequestHandler<UserIsOptedDownRequest, Activity>
    {
        public async Task<Activity> Handle(UserIsOptedDownRequest request, CancellationToken cancellationToken)
        {
            var optOutReply = request.Activity.CreateReply();
            optOutReply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.UserIsOptedDownText,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.ResumePairingsButtonText,
                            DisplayText = Resources.ResumePairingsButtonText,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.Optin
                        }
                    }
                }.ToAttachment(),
            };

            return optOutReply;
        }
    }
}