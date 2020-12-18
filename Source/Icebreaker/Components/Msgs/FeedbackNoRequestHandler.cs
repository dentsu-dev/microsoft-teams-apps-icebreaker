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

            var searchDate = DateTime.UtcNow.AddDays(-1 * Constants.FeedBackDelayDays);
            var lastUserMatch =
                await _repository.UserMatchInfoSearchByDateAndUser(
                    searchDate,
                    activity.From.AsTeamsChannelAccount().Email);

            var lastCompanionEmail = string.Empty;
            if (lastUserMatch != null)
            {
                lastCompanionEmail = lastUserMatch.RecipientEmail;
            }

            await _repository.FeedbackRootCreate(
                activity.From.AsTeamsChannelAccount().Email,
                lastCompanionEmail,
                FbRootTypes.No);

            return reply;
        }
    }
}