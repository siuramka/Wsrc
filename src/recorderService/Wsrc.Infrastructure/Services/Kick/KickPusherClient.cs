using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Wsrc.Domain;
using Wsrc.Infrastructure.Configuration;
using Wsrc.Infrastructure.Interfaces;

namespace Wsrc.Infrastructure.Services.Kick;

public class KickPusherClient(
    IOptions<KickConfiguration> kickConfig)
    : IKickPusherClient
{
    public required string ChatRoomId { get; init; }

    private readonly ClientWebSocket _socketClient = new();

    public async Task ConnectAsync(KickChatConnectionRequest connectionRequest)
    {
        await _socketClient.ConnectAsync(new Uri(kickConfig.Value.PusherConnectionString), CancellationToken.None);

        var connectionData = JsonSerializer.Serialize(connectionRequest);
        await Send(connectionData);
    }

    public async Task CloseAsync()
    {
        await _socketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    }

    public async Task Send(string data)
    {
        await _socketClient.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true,
            CancellationToken.None);
    }

    public async Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token)
    {
        return await _socketClient.ReceiveAsync(buffer, token);
    }
}