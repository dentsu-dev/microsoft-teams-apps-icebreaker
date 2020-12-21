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
    public class FeedBackNoNoGoodTimeRequestHandler : IRequestHandler<FeedBackNoNoGoodTimeRequest, Activity>
    {
        private readonly BotRepository _repository;

        public FeedBackNoNoGoodTimeRequestHandler(BotRepository repository)
        {
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedBackNoNoGoodTimeRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackNoNoGoodTimeReplyText
                }.ToAttachment(),
            };

            await _repository.FeedbackDetailCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbDetailTypes.NoGoodTime,
                FbRootTypes.No);

            return reply;
        }
    }
}