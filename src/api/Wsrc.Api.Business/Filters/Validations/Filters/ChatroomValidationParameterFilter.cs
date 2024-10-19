using Microsoft.AspNetCore.Http;
using Wsrc.Api.Business.Interfaces;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Filters.Validations.Filters;

public class ChatroomValidationParameterFilter(ValidationUtilities validationUtilities) 
    : IEndpointFilter,
    IParameterFilter<ChatroomSearchDto>
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
        var requiredError = validationUtilities
            .Required(chatroom.Username, nameof(chatroom.Username));

        var maxLengthError = validationUtilities
            .MaxLength(chatroom.Username, nameof(chatroom.Username), ValidationConstants.MaxNameLength);

        var errors = new List<string?> { requiredError, maxLengthError };

        return validationUtilities.GetErrors(errors);
    }
}