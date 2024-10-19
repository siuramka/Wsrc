using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wsrc.Api.Business.Filters.Validations.Filters;
using Wsrc.Api.Business.Services;
using Wsrc.Domain.Models.Chatmessages;
using Wsrc.Domain.Models.Chatmessages.Parameters;

namespace Wsrc.Api.Endpoints;

public class MessageEndpoints
{
    public static void RegisterEndpoints(RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("chatrooms/{channel}/messages", GetAll)
            .AddEndpointFilter<MessageGetAllParametersValidationFilter>();
    }

    public static async Task<Ok<IEnumerable<MessageDto>>> GetAll(
        [AsParameters] MessageGetAllParameters chatroomSearchDto,
        [FromServices] MessageService messageService)
    {
        var messagesDto = await messageService
            .GetAllAsync(chatroomSearchDto.Channel, chatroomSearchDto.SenderUsername);

        return TypedResults.Ok(messagesDto);
    }
}