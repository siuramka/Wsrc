using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using Wsrc.Core.Interfaces;
using Wsrc.Domain;
using Wsrc.Domain.Models;
using Wsrc.Infrastructure.Configuration;

namespace Wsrc.Infrastructure.Services;

public class KickPusherClient(
    IOptions<KickConfiguration> kickConfig)
    : IKickPusherClient
{
    private readonly ClientWebSocket _socketClient = new();

    public required string ChannelName { get; init; }

    public int ChannelId { get; init; }

    public async Task ConnectAsync()
    {
        await _socketClient.ConnectAsync(new Uri(kickConfig.Value.PusherConnectionString), CancellationToken.None);
    }

    public async Task SubscribeAsync(KickChatConnectionRequest connectionRequest)
    {
        var connectionData = JsonSerializer.Serialize(connectionRequest);
        await SendAsync(connectionData);
    }

    public async Task CloseAsync()
    {
        await _socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }

    public async Task SendAsync(string data)
    {
        await _socketClient.SendAsync(
            Encoding.UTF8.GetBytes(data),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
    {
        return await _socketClient.ReceiveAsync(buffer, token);
    }

    public override bool Equals(object? obj)
    {
        return obj is KickPusherClient client && ChannelId == client.ChannelId;
    }

    public override int GetHashCode()
    {
        return ChannelId;
    }
}