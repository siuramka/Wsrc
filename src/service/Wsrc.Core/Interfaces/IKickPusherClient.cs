using System.Net.WebSockets;

using Wsrc.Domain;

namespace Wsrc.Core.Interfaces;

public interface IKickPusherClient
{
    public string ChannelName { get; init; }

    public int ChannelId { get; init; }

    Task ConnectAsync();

    Task SubscribeAsync(KickChatConnectionRequest connectionRequest);

    Task CloseAsync();

    Task Send(string data);

    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token);
}