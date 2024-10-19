using Microsoft.AspNetCore.Http;
using Wsrc.Api.Business.Interfaces;
using Wsrc.Domain.Models.Chatmessages.Parameters;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Filters.Validations.Filters;

public class MessageGetAllParametersValidationFilter(ValidationUtilities validationUtilities)
    : IEndpointFilter,
        IParametersValidationFilter<MessageGetAllParameters>
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var chatroomSearchDto = context.GetArgument<MessageGetAllParameters>(0);

        var validationErrors = GetValidationErrors(chatroomSearchDto);

        if (!string.IsNullOrEmpty(validationErrors))
        {
            return Results.Problem(validationErrors);
        }

        return await next(context);
    }

    public string GetValidationErrors(MessageGetAllParameters parameters)
    {
        var requiredChannelError = validationUtilities
            .Required(parameters.Channel, nameof(parameters.Channel));

        var maxLengthChannelError = validationUtilities
            .MaxLength(parameters.Channel, nameof(parameters.Channel), ValidationConstants.MaxNameLength);

        if (string.IsNullOrEmpty(parameters.SenderUsername))
        {
            return validationUtilities
                .GetErrors([requiredChannelError, maxLengthChannelError]);
        }

        var requiredSenderError = validationUtilities
            .Required(parameters.SenderUsername, nameof(parameters.SenderUsername));

        var maxLengthSenderError = validationUtilities
            .MaxLength(parameters.SenderUsername, nameof(parameters.SenderUsername), ValidationConstants.MaxNameLength);

        return validationUtilities
            .GetErrors([requiredChannelError, maxLengthChannelError, requiredSenderError, maxLengthSenderError]);
    }
}