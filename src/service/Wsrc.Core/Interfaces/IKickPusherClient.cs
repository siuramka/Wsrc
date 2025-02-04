using System.Net.WebSockets;

using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Core.Interfaces;

public interface IKickPusherClient
{
    public string ChannelName { get; init; }

    public int ChannelId { get; init; }

    Task ConnectAsync();

    Task SubscribeAsync(KickChatConnectionRequest connectionRequest);

    Task CloseAsync();

    Task SendAsync(string data);

    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token);
}