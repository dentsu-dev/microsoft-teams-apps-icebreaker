using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Db;
using Icebreaker.Properties;
using MediatR;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components.Msgs
{
    public class FeedBackNoCanceledRequestHandler : IRequestHandler<FeedBackNoCanceledRequest, Activity>
    {
        private readonly BotRepository _repository;

        public FeedBackNoCanceledRequestHandler(BotRepository repository)
        {
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedBackNoCanceledRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackNoCanceledReplyText
                }.ToAttachment(),
            };
            
            await _repository.FeedbackDetailCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbDetailTypes.Canceled,
                FbRootTypes.No);

            return reply;
        }
    }
}