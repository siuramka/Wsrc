using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wsrc.Api.Business.Filters.Validations.Filters;
using Wsrc.Api.Business.Services;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Endpoints;

public static class ChatroomEndpoints
{
    public static void RegisterEndpoints(RouteGroupBuilder routeBuilder)
    {
        routeBuilder.MapGet("/chatrooms", GetAll)
            .AddEndpointFilter<ChatroomGetAllParametersValidationFilter>();
    }

    public static async Task<Ok<IEnumerable<ChatroomDto>>> GetAll(
        [AsParameters] ChatroomGetAllParameters chatroomGetAllParameters,
        [FromServices] ChatroomService chatroomService)
    {
        var chatroomsDto = await chatroomService.GetAllAsync(chatroomGetAllParameters.Username);

        return TypedResults.Ok(chatroomsDto);
    }
}