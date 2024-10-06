namespace Wsrc.Infrastructure.Configuration;

public class RabbitMqConfiguration
{
    public const string Section = "RabbitMq";

    public string HostName { get; init; }

    public string Username { get; init; }

    public string Password { get; init; }
}
