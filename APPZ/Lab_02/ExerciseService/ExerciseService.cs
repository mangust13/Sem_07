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
        // Логування
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .WriteTo.File(
                path: "exercise_log.txt", 
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

            // 🧩 Обмінник Topic
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

            // ====== Queue для аудіо
            await channel.QueueDeclareAsync("exercise.audio", false, false, false, null);
            await channel.QueueBindAsync("exercise.audio", exchangeName, "exercise.audio.new");

            // ====== Queue для результатів
            await channel.QueueDeclareAsync("speech.result", false, false, false, null);
            await channel.QueueBindAsync("speech.result", exchangeName, "speech.result.*");

            // ====== Consumer для результатів ======
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var result = JsonSerializer.Deserialize<JsonElement>(json);

                    var exerciseId = result.GetProperty("ExerciseId").GetString();
                    var accuracy = result.GetProperty("Accuracy").GetDouble();
                    var feedback = result.GetProperty("Feedback").GetString();

                    Log.Information("📥 Отримано результат для вправи #{Id}: {Accuracy:P0} | {Feedback}", exerciseId, accuracy, feedback);
                    await Task.Delay(500);
                    Log.Information("[ExerciseService] Результат збережено у ProgressService ✅");

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nВведи ідентифікатор вправи (або 'exit' для виходу):");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "❌ Помилка при обробці результату");
                }
            };

            await channel.BasicConsumeAsync("speech.result", autoAck: true, consumer);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Введи ідентифікатор вправи (або 'exit' для виходу):");
            Console.ResetColor();

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

                    Log.Information("[ExerciseService] Надіслано аудіо для вправи {Id}", exerciseId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "❌ Помилка при надсиланні повідомлення");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "❌ Помилка при запуску ExerciseService");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
