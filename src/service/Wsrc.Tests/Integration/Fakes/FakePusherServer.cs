using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

using Microsoft.AspNetCore.Builder;

using Wsrc.Domain;
using Wsrc.Domain.Models;

namespace Wsrc.Tests.Integration.Fakes;

public class FakePusherServer
{
    public readonly List<WebSocket> ActiveConnections = [];
    public WebApplication App = null!;

    public async Task StartAsync()
    {
        var builder = WebApplication.CreateBuilder();
        App = builder.Build();

        App.UseWebSockets();

        App.Map("/app/{key}",
            async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    await HandleConnectionAsync(webSocket);
                }
            });

        await App.StartAsync();
    }

    public static string GetConnectionString()
    {
        return "ws://localhost:5000/app/1234";
    }

    private async Task HandleConnectionAsync(WebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            var message = await GetMessageAsync(webSocket);
            var kickEvent = JsonSerializer.Deserialize<KickEvent>(message);
            var pusherEvent = PusherEvent.Parse(kickEvent!.Event);

            if (pusherEvent.Event == PusherEvent.Connected.Event)
            {
                var connectionEstablished = new
                {
                    @event = PusherEvent.Connected.Event,
                };

                await SendMessageAsync(webSocket, connectionEstablished);
            }

            if (pusherEvent!.Event == PusherEvent.Subscribe.Event)
            {
                ActiveConnections.Add(webSocket);
            }
        }
    }

    private static async Task<string> GetMessageAsync(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

    private static async Task SendMessageAsync(WebSocket webSocket, object message)
    {
        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);

        await webSocket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}