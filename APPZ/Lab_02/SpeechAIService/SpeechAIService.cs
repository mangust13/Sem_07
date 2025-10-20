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
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .WriteTo.File(
                path: "speechai_log.txt", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger();

        try
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            const string exchangeName = "speech_exchange";

            // 🧩 Обмінник Topic
            await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic);

            // Queue для прийому аудіо
            await channel.QueueDeclareAsync("exercise.audio", false, false, false, null);
            await channel.QueueBindAsync("exercise.audio", exchangeName, "exercise.audio.*");

            Log.Information("[SpeechAIService] Очікування повідомлень...");

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<JsonElement>(json);

                    var exerciseId = message.GetProperty("ExerciseId").GetString();
                    Log.Information("🎧 Отримано вправу #{Id}, аналіз вимови...", exerciseId);

                    await Task.Delay(2000); // Імітація аналізу

                    var result = new
                    {
                        ExerciseId = exerciseId,
                        Accuracy = 0.91,
                        Feedback = "Вимова правильна, артикуляція гарна!",
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

                    Log.Information("✅ Результат для вправи #{Id} опубліковано у 'speech.result.done'", exerciseId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "❌ Помилка при обробці повідомлення");
                }
            };

            await channel.BasicConsumeAsync("exercise.audio", autoAck: true, consumer);

            Console.WriteLine("Натисни Enter для виходу...");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "❌ Помилка при запуску SpeechAIService");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
