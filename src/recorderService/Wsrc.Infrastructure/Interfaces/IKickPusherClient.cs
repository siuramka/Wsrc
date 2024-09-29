using System.Net.WebSockets;
using Wsrc.Domain;

namespace Wsrc.Infrastructure.Interfaces;

public interface IKickPusherClient
{
    string ChatRoomId { get; init; }

    Task ConnectAsync(KickChatConnectionRequest connectionRequest);

    Task CloseAsync();

    Task Send(string data);

    Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken token);
}
