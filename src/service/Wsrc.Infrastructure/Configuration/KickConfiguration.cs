using Wsrc.Domain;

namespace Wsrc.Infrastructure.Configuration;

public class KickConfiguration
{
    public const string Section = "Kick";

    public required string PusherConnectionString { get; init; }

    public IEnumerable<KickChannel> Channels { get; init; } = [];
}