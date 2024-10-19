namespace Wsrc.Api.Endpoints;

public class EndpointConfigurator
{
    public static void Configure(WebApplication app)
    {
        var apiGroup = app.MapGroup("/api/v1");
        ChatroomEndpoints.RegisterEndpoints(apiGroup);
        MessageEndpoints.RegisterEndpoints(apiGroup);
    }
}