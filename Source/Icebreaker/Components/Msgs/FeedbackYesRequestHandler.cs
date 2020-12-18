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
using Microsoft.Bot.Connector.Teams.Models;

namespace Icebreaker.Components.IncomingMsgs
{
    public class FeedbackYesRequestHandler : IRequestHandler<FeedbackYesRequest, Activity>
    {
        private readonly TelemetryClient _telemetryClient;
        private readonly IcebreakerBot _bot;
        private readonly BotRepository _repository;

        public FeedbackYesRequestHandler(TelemetryClient telemetryClient, IcebreakerBot bot, BotRepository repository)
        {
            _telemetryClient = telemetryClient;
            _bot = bot;
            _repository = repository;
        }

        public async Task<Activity> Handle(FeedbackYesRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;

            //var senderAadId = activity.From.Properties["aadObjectId"].ToString();
            //var tenantId = activity.GetChannelData<TeamsChannelData>().Tenant.Id;

            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>
            {
                new HeroCard()
                {
                    Text = Resources.FeedBackYesAnswerRequestText,
                    Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title = Resources.FeedBackYesAnswerVeryGood,
                            DisplayText = Resources.FeedBackYesAnswerVeryGood,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedbackYesSuper
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackYesAnswerGood,
                            DisplayText = Resources.FeedBackYesAnswerGood,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedbackYesGood
                        },
                        new CardAction()
                        {
                            Title = Resources.FeedBackYesAnswerBad,
                            DisplayText = Resources.FeedBackYesAnswerBad,
                            Type = ActionTypes.MessageBack,
                            Text = CardActions.FeedbackYesBad
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
                FbRootTypes.Yes);

            return reply;
        }
    }
}