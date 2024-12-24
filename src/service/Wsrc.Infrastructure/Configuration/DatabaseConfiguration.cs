using System.Dynamic;

using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Configuration;

public class DatabaseConfiguration
{
    public const string Section = "Database";

    public required string PostgresEfCoreConnectionString { get; init; }
}