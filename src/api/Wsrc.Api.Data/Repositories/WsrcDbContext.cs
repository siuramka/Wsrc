using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Wsrc.Domain.Repositories;

public class WsrcDbContext(IConfiguration configuration) : DbContext
{
    private const string ConnectionConfigKey = "Postgresql";

    public DbSet<Chatroom> Chatrooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Sender> Senders { get; set; }

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString(ConnectionConfigKey));
    }
}