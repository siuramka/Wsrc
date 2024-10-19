namespace Wsrc.Api.Endpoints;

public class EndpointConfigurator
{
    public static void Configure(WebApplication app)
    {
        ChatroomEndpoints.RegisterEndpoints(app);
    }
}