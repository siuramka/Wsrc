using Microsoft.EntityFrameworkCore;

using Wsrc.Domain.Entities;

namespace Wsrc.Infrastructure.Persistence;

public class WsrcContext(DbContextOptions<WsrcContext> options) : DbContext(options)
{
    public DbSet<Chatroom> Chatrooms { get; set; }

    public DbSet<Message> Messages { get; set; }

    public DbSet<Sender> Senders { get; set; }

    public DbSet<Channel> Channels { get; set; }
}