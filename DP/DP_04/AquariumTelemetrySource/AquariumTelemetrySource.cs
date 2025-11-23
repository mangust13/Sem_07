using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace AquariumTelemetrySource;

class AquariumTelemetrySource
{
    static async Task Main()
    {
        Console.WriteLine("Aquarium telemetry source running...");
        var server = new HttpListener();
        server.Prefixes.Add("http://localhost:5055/ws/");
        server.Start();

        while(true)
        {
            var ctx = await server.GetContextAsync();
            if (ctx.Request.IsWebSocketRequest)
            {
                var wsContext = await ctx.AcceptWebSocketAsync(null);
                Task.Run(() => SendTelemetry(wsContext.WebSocket));
            }
            else
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.Close();
            }
        }
    }

    static async Task SendTelemetry(WebSocket socket)
    {
        var random = new Random();

        while (socket.State == WebSocketState.Open)
        {
            var data = new
            {
                Temperature = Math.Round(15 + random.NextDouble() * 20, 1), //15 - 35
                PH = Math.Round(6.5 + random.NextDouble() * 2, 1),
                Light = random.Next(0, 101),                                
                FeedLevel = random.Next(0, 101),                             
                Activity = random.Next(0, 100),
                Timestamp = DateTime.UtcNow
            };


            var json = JsonSerializer.Serialize(data);
            Console.WriteLine(json);

            var bytes = Encoding.UTF8.GetBytes(json);

            await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(1000);
        }
    }
}
