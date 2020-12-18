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
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;

namespace Icebreaker.Components
{
    public class HandleMessageRequestHander : IRequestHandler<HandleMessageRequest>
    {
        private readonly IMediator _mediator;
        private readonly BotRepository _repository;

        public HandleMessageRequestHander(IMediator mediator, BotRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        public async Task<Unit> Handle(HandleMessageRequest request, CancellationToken cancellationToken)
        {
            var activity = request.Activity;
            var connectorClient = request.connectorClient;

            Activity reply;
            string botLastMessage;

            if (activity.Is(CardActions.Optout))
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
                reply = await _mediator.Send(new FeedbackYesRequest { Activity = activity });
                botLastMessage = BotMessageTypes.FbRootYesDetails;
            }
            else if (activity.Is(CardActions.FeedbackNo))
            {
                reply = await _mediator.Send(new FeedbackNoRequest { Activity = activity });
                botLastMessage = BotMessageTypes.FbRootNoDetails;
            }
            else if (activity.Is(CardActions.FeedbackYesSuper))
            {
                reply = await _mediator.Send(new FeedbackYesSuperReplyRequest { Activity = activity });
                botLastMessage = BotMessageTypes.FbYesSuperResponse;
            }
            else if (activity.Is(CardActions.FeedbackYesGood))
            {
                reply = await _mediator.Send(new FeedBackYesGoodReplyRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbYesGoodResponse;
            }
            else if (activity.Is(CardActions.FeedbackYesBad))
            {
                reply = await _mediator.Send(new FeedbackYesBadRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbYesBadResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoNoResponse))
            {
                reply = await _mediator.Send(new FeedbackNoNoResponseRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbNoMeetupResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoCanceled))
            {
                reply = await _mediator.Send(new FeedBackNoCanceledRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbNoCanceledResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoNoGoodTime))
            {
                reply = await _mediator.Send(new FeedBackNoNoGoodTimeRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbNoGoodTimeResponse;
            }
            else if (activity.Is(CardActions.FeedBackNoAnother))
            {
                reply = await _mediator.Send(new FeedBackNoAnotherRequest() { Activity = activity });
                botLastMessage = BotMessageTypes.FbNoAnotherReasonResponse;
            }
            else
            {
                var commonRes = await _mediator.Send(new CommonTextRequest { Activity = activity });
                reply = commonRes.Reply;
                botLastMessage = commonRes.NewBotMessage;
            }

            if (reply != null)
            {
                await connectorClient.Conversations.ReplyToActivityAsync(reply, cancellationToken);
                await _repository.BotLastMessageUpdate(activity.From.AsTeamsChannelAccount().Email, botLastMessage);
            }

            return Unit.Value;
        }
    }
}