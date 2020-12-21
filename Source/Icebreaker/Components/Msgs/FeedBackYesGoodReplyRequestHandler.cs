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

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedBackYesGoodReplyRequestHandler : IRequestHandler<FeedBackYesGoodReplyRequest, Activity>
    {
        private readonly BotRepository _repository;

        public FeedBackYesGoodReplyRequestHandler(BotRepository repository)
        {
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedBackYesGoodReplyRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackYesGoodReplyText1 + Environment.NewLine + Resources.FeedBackYesGoodReplyText2
                }.ToAttachment(),
            };

            await _repository.FeedbackDetailCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbDetailTypes.Good,
                FbRootTypes.Yes);

            return reply;
        }
    }
}