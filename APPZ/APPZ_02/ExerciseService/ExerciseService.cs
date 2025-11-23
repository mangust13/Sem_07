using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using Serilog.Events;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExerciseService;

class ExerciseService
{
    static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(theme: Serilog.Sinks.SystemConsole.Themes.ConsoleTheme.None,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .WriteTo.File("exercise_log.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();

        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            const string exchangeName = "speech_exchange";
            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Topic);

            // declare queues (for results)
            await channel.QueueDeclareAsync("speech.result", false, false, false, null);
            await channel.QueueBindAsync("speech.result", exchangeName, "speech.result.*");

            // subscribe to results
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var result = JsonSerializer.Deserialize<JsonElement>(json);
                    var exerciseId = result.GetProperty("ExerciseId").GetString();
                    var accuracy = result.GetProperty("Accuracy").GetDouble() * 100;
                    var feedback = result.GetProperty("Feedback").GetString();

                    Log.Information("Received result for exercise #{Id}: {Accuracy:F1}% | {Feedback}",
                        exerciseId, accuracy, feedback);
                    await Task.Delay(800);
                    Log.Information("[ExerciseService] Result saved in ProgressService.");
                    Console.WriteLine("Enter exercise ID (or 'exit' to quit):");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error processing result");
                }
            };

            await channel.BasicConsumeAsync("speech.result", autoAck: true, consumer);

            Console.WriteLine("Enter exercise ID (or 'exit' to quit):");

            while (true)
            {
                var exerciseId = Console.ReadLine();
                if (exerciseId == "exit") break;

                try
                {
                    var message = new
                    {
                        ExerciseId = exerciseId,
                        UserId = 7,
                        AudioUrl = $"audio_{exerciseId}.wav",
                        Timestamp = DateTime.UtcNow
                    };

                    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    var props = new BasicProperties();

                    await channel.BasicPublishAsync<BasicProperties>(
                        exchange: exchangeName,
                        routingKey: "exercise.audio.new",
                        mandatory: false,
                        basicProperties: props,
                        body: body);

                    Log.Information("[ExerciseService] Sent audio request for exercise #{Id}", exerciseId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while sending message");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Critical error while running ExerciseService");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
