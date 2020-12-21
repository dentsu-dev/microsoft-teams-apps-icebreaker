using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Db;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedbackYesSuperReplyRequestHandler : IRequestHandler<FeedbackYesSuperReplyRequest, Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;
        private readonly BotRepository _repository;

        public FeedbackYesSuperReplyRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot, BotRepository repository)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedbackYesSuperReplyRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedbackYesSuperReplyText1 + Environment.NewLine + Resources.FeedbackYesSuperReplyText2
                }.ToAttachment(),
            };

            await _repository.FeedbackDetailCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbDetailTypes.Super,
                FbRootTypes.Yes);

            return reply;
        }
    }
}