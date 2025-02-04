using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;

using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Tests.Integration.Reusables.Fakes;

public class FakePusherServer : IAsyncDisposable
{
    public readonly Dictionary<WebSocket, CancellationTokenSource> ActiveConnections = [];
    private WebApplication _app = null!;

    public async Task StartAsync()
    {
        var builder = WebApplication.CreateBuilder();
        _app = builder.Build();

        _app.UseWebSockets();

        _app.Map("/app/{key}",
            async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    await HandleConnectionAsync(webSocket);
                }
            });

        await _app.StartAsync();
    }

    public static string GetConnectionString()
    {
        return "ws://localhost:5000/app/1234";
    }

    public async Task DropConnectionsAsync()
    {
        foreach (var connection in ActiveConnections)
        {
            await connection.Key.CloseOutputAsync(
                WebSocketCloseStatus.NormalClosure,
                "integrationTest:close",
                CancellationToken.None);

            await connection.Value.CancelAsync();
        }

        ActiveConnections.Clear();
    }

    private async Task HandleConnectionAsync(WebSocket webSocket)
    {
        var cts = new CancellationTokenSource();

        while (webSocket.State == WebSocketState.Open)
        {
            var message = await GetMessageAsync(webSocket, cts);
            var kickEvent = JsonSerializer.Deserialize<KickEvent>(message);
            var pusherEvent = PusherEvent.Parse(kickEvent!.Event);

            if (pusherEvent.Event == PusherEvent.Connected.Event)
            {
                var connectionEstablished = new { @event = PusherEvent.Connected.Event, };

                await SendMessageAsync(webSocket, connectionEstablished);
            }

            if (pusherEvent!.Event == PusherEvent.Subscribe.Event)
            {
                ActiveConnections.Add(webSocket, cts);
            }
        }
    }

    private static async Task<string> GetMessageAsync(WebSocket webSocket, CancellationTokenSource cts)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
        catch (TaskCanceledException _)
        {
            return string.Empty;
        }
    }

    public async Task SendMessageAsync(WebSocket webSocket, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);

        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    public async Task SendMessageAsync(WebSocket webSocket, string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);

        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    public async Task StopAsync()
    {
        await _app.StopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _app.DisposeAsync();
    }
}