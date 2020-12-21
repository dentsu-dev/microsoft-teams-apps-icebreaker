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
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Components.Msgs
{
    public class FeedBackNoAnotherRequestHandler : IRequestHandler<FeedBackNoAnotherRequest, Activity>
    {
        private readonly BotRepository _repository;

        public FeedBackNoAnotherRequestHandler(BotRepository repository)
        {
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedBackNoAnotherRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackNoAnotherReplyText
                }.ToAttachment(),
            };
            
            await _repository.FeedbackDetailCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbDetailTypes.NoAnother,
                FbRootTypes.No);
            
            return reply;
        }
    }
}