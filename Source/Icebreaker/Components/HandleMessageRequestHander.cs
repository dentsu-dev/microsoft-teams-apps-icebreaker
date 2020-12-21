using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Icebreaker.Components.IncomingMsgs;
using Icebreaker.Components.Msgs;
using Icebreaker.Db;
using Icebreaker.Helpers;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components
{
    public class HandleMessageRequestHander : IRequestHandler<HandleMessageRequest>
    {
        private readonly IMediator _mediator;
        private readonly BotRepository _repository;
        private readonly IcebreakerBot _bot;
        private readonly TelemetryClient _telemetryClient;

        public HandleMessageRequestHander(IMediator mediator, BotRepository repository, IcebreakerBot bot, TelemetryClient telemetryClient)
        {
            _mediator = mediator;
            _repository = repository;
            _bot = bot;
            _telemetryClient = telemetryClient;
        }

        public async Task<Unit> Handle(HandleMessageRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var connectorClient = request.connectorClient;

            Activity reply;
            string botLastMessage;

            var senderAadId = activity.SenderAadId();

            var senderUserDetails = await _repository.UserDetailsGet(senderAadId);
            var lastMatch = await _bot.GetLastMatch(senderAadId);

            var lastBotMessageInfo = await _repository.BotLastMessageGet(senderAadId);

            var userOptStatusInfo = await _repository.UserInfoGet(senderAadId);

            if (senderUserDetails == null || lastBotMessageInfo == null)
            {
                reply = await _mediator.Send(new UnregisterdUserRequest {Activity = activity});
                botLastMessage = BotMessageTypes.UnregisteredUser;
            }
            else if (lastMatch == null && userOptStatusInfo != null && userOptStatusInfo.OptedIn == false)
            {
                reply = await _mediator.Send(new UserIsOptedDownRequest { Activity = activity });
                botLastMessage = BotMessageTypes.UnregisteredUser;
            }
            else if (lastMatch == null)
            {
                _telemetryClient.TrackTrace($"User {senderUserDetails.Email} has last time math");
                throw new Exception($"User {senderUserDetails.Email} has last time math");
            }
            else if (activity.Is(CardActions.Optout))
            {
                reply = await _mediator.Send(new OptOutRequest { Activity = activity });
                botLastMessage = BotMessageTypes.OptOut;
            }
            else if (activity.Is(CardActions.Optin))
            {
                reply = await _mediator.Send(new OptInRequest { Activity = activity });
                botLastMessage = BotMessageTypes.OptIn;
            }
            else if (activity.Is(CardActions.FeedbackYes))
            {
                reply = await _mediator.Send(new FeedbackYesRequest { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbRootYesDetails;
            }
            else if (activity.Is(CardActions.FeedbackNo))
            {
                reply = await _mediator.Send(new FeedbackNoRequest { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbRootNoDetails;
            }
            else if (activity.Is(CardActions.FeedbackYesSuper))
            {
                reply = await _mediator.Send(new FeedbackYesSuperReplyRequest { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbYesSuperResponse;
            }
            else if (activity.Is(CardActions.FeedbackYesGood))
            {
                reply = await _mediator.Send(new FeedBackYesGoodReplyRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbYesGoodResponse;
            }
            else if (activity.Is(CardActions.FeedbackYesBad))
            {
                reply = await _mediator.Send(new FeedbackYesBadRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbYesBadResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoNoResponse))
            {
                reply = await _mediator.Send(new FeedbackNoNoResponseRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbNoMeetupResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoCanceled))
            {
                reply = await _mediator.Send(new FeedBackNoCanceledRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbNoCanceledResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoNoGoodTime))
            {
                reply = await _mediator.Send(new FeedBackNoNoGoodTimeRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbNoGoodTimeResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoAnother))
            {
                reply = await _mediator.Send(new FeedBackNoAnotherRequest() { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                botLastMessage = BotMessageTypes.FbNoAnotherReasonResponse;
            }
            else
            {
                var commonRes = await _mediator.Send(new CommonTextRequest { Activity = activity, UserMatch = lastMatch, BotLastMessage = lastBotMessageInfo });
                reply = commonRes.Reply;
                botLastMessage = commonRes.NewBotMessage;
            }

            if (reply != null)
            {
                await connectorClient.Conversations.ReplyToActivityAsync(reply, cancellationToken);
                await _repository.BotLastMessageUpdate(senderAadId, senderUserDetails?.Email, botLastMessage);
            }

            return Unit.Value;
        }
    }
}