namespace Wsrc.Infrastructure.Configuration;

public class RabbitMqConfiguration
{
    public const string Section = "RabbitMq";

    public required string HostName { get; init; }

    public required string Username { get; init; }

    public required string Password { get; init; }
}