using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Wsrc.Api.Business.Filters.Validations;
using Wsrc.Api.Business.Filters.Validations.Filters;
using Wsrc.Api.Business.Services;
using Wsrc.Api.Business.Services.Mappings;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Endpoints;

public static class ChatroomEndpoints
{
    public static void RegisterEndpoints(WebApplication app)
    {
        app.MapGet("/chatrooms", GetAll)
            .AddEndpointFilter<ChatroomValidationParameterFilter>();
    }

    public static async Task<Ok<IEnumerable<ChatroomDto>>> GetAll(
        [AsParameters] ChatroomSearchDto chatroomSearchDto,
        [FromServices] ChatroomService chatroomService)
    {
        var chatrooms = await chatroomService.GetAllAsync(chatroomSearchDto.Username);

        return TypedResults.Ok(ChatroomMapper.ToDtoList(chatrooms));
    }
}