using Microsoft.AspNetCore.Http;
using Wsrc.Api.Business.Interfaces;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Filters.Validations.Filters;

public class ChatroomSearchDtoValidationFilter(ValidationUtilities validationUtilities)
    : IEndpointFilter,
        IParametersValidationFilter<ChatroomSearchDto>
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var chatroomSearchDto = context.GetArgument<ChatroomSearchDto>(0);

        var validationErrors = GetValidationErrors(chatroomSearchDto);

        if (!string.IsNullOrEmpty(validationErrors))
        {
            return Results.Problem(validationErrors);
        }

        return await next(context);
    }

    public string GetValidationErrors(ChatroomSearchDto chatroom)
    {
        if (chatroom.Username is null)
        {
            return string.Empty;
        }

        var maxLengthError = validationUtilities
            .MaxLength(chatroom.Username, nameof(chatroom.Username), ValidationConstants.MaxNameLength);

        var errors = new List<string?> { maxLengthError };

        return validationUtilities.GetErrors(errors);
    }
}