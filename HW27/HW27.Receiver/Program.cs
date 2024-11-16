using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

class Program
{
	static void Main(string[] args)
	{
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnectionAsync())
		using (var channel = connection.Result.CreateChannelAsync())
		{
			string exchangeName = "chat_exchange";
			channel.Result.ExchangeDeclareAsync(exchange: exchangeName, type: "direct");

			Console.WriteLine("Enter your username:");
			string username = Console.ReadLine();


			channel.Result.QueueDeclareAsync(
				queue: "userQueue",
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);


			channel.Result.QueueBindAsync(
				queue: "userQueue",
				exchange: exchangeName,
				routingKey: username);


			var consumer = new AsyncEventingBasicConsumer(channel.Result);
			consumer.ReceivedAsync += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"[Received] {message}");

				await Task.CompletedTask;
			};

			channel.Result.BasicConsumeAsync(
				queue: "userQueue",
				autoAck: true,
				consumer: consumer);

			string message = "Hello RabbitMQ";
			var body = Encoding.UTF8.GetBytes(message);

			channel.Result.BasicPublishAsync(
				exchange: "",
				routingKey: "userQueue",
				body: body);

			Console.WriteLine($" [x] Sent {message}");
		}

		Console.ReadLine();
	}
}
