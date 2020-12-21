using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Icebreaker.Db;
using Icebreaker.Properties;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedbackNoRequestHandler : IRequestHandler<FeedbackNoRequest,Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;
        private readonly BotRepository _repository;

        public FeedbackNoRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot, BotRepository repository)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedbackNoRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var botMsg = request.BotLastMessage.Message;
            var lastMatch = request.UserMatch;

            var reply = request.Activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackNoAnswerRequestText,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.FeedBackNoAnswerNoResponse,
                            DisplayText = Resources.FeedBackNoAnswerNoResponse,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedBackNoNoResponse
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackYesAnswerCanceled,
                            DisplayText = Resources.FeedBackYesAnswerCanceled,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedBackNoCanceled
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackNoAnswerNoGoodTime,
                            DisplayText = Resources.FeedBackNoAnswerNoGoodTime,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedBackNoNoGoodTime
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackNoAnswerAnother,
                            DisplayText = Resources.FeedBackNoAnswerAnother,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedBackNoAnother
                        }
                    }
                }.ToAttachment(),
            };

            await _repository.FeedbackRootCreate(
                lastMatch.SenderEmail,
                lastMatch.SenderAadId,
                lastMatch.RecipientEmail,
                lastMatch.RecipientAadId,
                FbRootTypes.No);

            return reply;
        }
    }
}