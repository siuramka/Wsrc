using Microsoft.AspNetCore.Http;
using Wsrc.Api.Business.Interfaces;
using Wsrc.Domain.Models.Chatrooms;

namespace Wsrc.Api.Business.Filters.Validations.Filters;

public class ChatroomGetAllParametersValidationFilter(ValidationUtilities validationUtilities)
    : IEndpointFilter,
        IParametersValidationFilter<ChatroomGetAllParameters>
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var chatroomSearchDto = context.GetArgument<ChatroomGetAllParameters>(0);

        var validationErrors = GetValidationErrors(chatroomSearchDto);

        if (!string.IsNullOrEmpty(validationErrors))
        {
            return Results.Problem(validationErrors);
        }

        return await next(context);
    }

    public string GetValidationErrors(ChatroomGetAllParameters parameters)
    {
        if (parameters.Username is null)
        {
            return string.Empty;
        }

        var maxLengthError = validationUtilities
            .MaxLength(parameters.Username, nameof(parameters.Username), ValidationConstants.MaxNameLength);
        
        return validationUtilities.GetErrors([maxLengthError]);
    }
}