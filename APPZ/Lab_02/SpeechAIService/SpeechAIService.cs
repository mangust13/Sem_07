using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpeechAIService;

class SpeechAIService
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.ConsoleTheme.None,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .WriteTo.File("speechai_log.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger();

        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            const string exchangeName = "speech_exchange";

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic);
            await channel.QueueDeclareAsync("exercise.audio", false, false, false, null);
            await channel.QueueBindAsync("exercise.audio", exchangeName, "exercise.audio.*");

            Log.Information("[SpeechAIService] Waiting for audio messages...");

            var random = new Random();
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<JsonElement>(json);
                    var exerciseId = message.GetProperty("ExerciseId").GetString();

                    Log.Information("Received exercise #{Id}. Running speech analysis...", exerciseId);

                    await Task.Delay(2000); // simulate AI analysis

                    var accuracy = random.NextDouble() * 100;
                    string feedback = accuracy switch
                    {
                        < 25 => "Poor pronunciation. Needs improvement.",
                        < 50 => "Fair, but articulation needs more work.",
                        < 75 => "Good pronunciation. Keep practicing!",
                        _ => "Excellent pronunciation! Clear and natural speech!"
                    };

                    var result = new
                    {
                        ExerciseId = exerciseId,
                        Accuracy = accuracy / 100.0,
                        Feedback = feedback,
                        Timestamp = DateTime.UtcNow
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(result));
                    var props = new BasicProperties();

                    await channel.BasicPublishAsync<BasicProperties>(
                        exchange: exchangeName,
                        routingKey: "speech.result.done",
                        mandatory: false,
                        basicProperties: props,
                        body: body);

                    Log.Information("Published analysis result for exercise #{Id}: {Accuracy:F1}% | {Feedback}",
                        exerciseId, accuracy, feedback);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while processing message");
                }
            };

            await channel.BasicConsumeAsync("exercise.audio", autoAck: true, consumer);

            Console.WriteLine("Press ENTER to exit...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Critical error while running SpeechAIService");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
