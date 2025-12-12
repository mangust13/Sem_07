using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;

namespace AquariumTelemetryConsumer;

public class TelemetryData
{
    public double Temperature { get; set; }
    public double PH { get; set; }
    public int Light { get; set; }
    public int FeedLevel { get; set; }
    public int Activity { get; set; }
    public DateTime Timestamp { get; set; }
}

class AquariumTelemetryConsumer
{
    static async Task Main()
    {
        Console.WriteLine("Aquarium monitor started...");
        var socket = new ClientWebSocket();
        await socket.ConnectAsync(new Uri("ws://localhost:5055/ws/"), CancellationToken.None);
        Console.WriteLine("Connected to AquariumTelemetrySource\n");

        var stream = Observable.Create<string>(async (observer, ct) =>
        {
            var buffer = new byte[1024];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer, ct);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    observer.OnCompleted();
                    break;
                }

                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                observer.OnNext(json);
            }
        });

        var telemetryStream = stream
            .Select(json => JsonSerializer.Deserialize<TelemetryData>(json)!)    // Map
            .Where(d => d != null && d.Temperature >= 22.0 && d.Temperature <= 30.0)                 // Filter
            .Buffer(TimeSpan.FromSeconds(4))                                     // Buffer (збираємо по 4 сек)
            .Where(batch => batch.Count > 0)
            .Select(batch => new
            {
                AvgTemp = batch.Average(d => d.Temperature),
                AvgPH = batch.Average(d => d.PH),
                AvgActivity = batch.Average(d => d.Activity),
                MaxLight = batch.Max(d => d.Light),
                MinFeed = batch.Min(d => d.FeedLevel)
            })                                                                  // Reduce (агрегація)
            .Take(20)                                                           // Take – обмежуємо кількість ітерацій
            .Do(summary =>
            {
                Console.WriteLine($"[Summary {DateTime.Now:T}]");
                Console.WriteLine($"  Temp avg: {summary.AvgTemp:F1}°C");
                Console.WriteLine($"  pH avg: {summary.AvgPH:F2}");
                Console.WriteLine($"  Activity avg: {summary.AvgActivity:F1}%");
                Console.WriteLine($"  Max light: {summary.MaxLight}% | Min feed: {summary.MinFeed}%\n");
            });

        await telemetryStream.ForEachAsync(_ => { });
    }
}
