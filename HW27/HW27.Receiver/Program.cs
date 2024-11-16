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
			var queueName = "userQueue";
			channel.Result.QueueDeclareAsync(
				queue: queueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);

            Console.WriteLine("Waiting for messages...");

			var consumer = new AsyncEventingBasicConsumer(channel.Result);

			consumer.ReceivedAsync += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine($"Received: {message}");

				await Task.CompletedTask;
			};

			channel.Result.BasicConsumeAsync(
				queue: queueName,
				autoAck: true,
				consumer: consumer);

			Console.ReadLine();
        }
	}
}
