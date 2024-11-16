using RabbitMQ.Client;
using System.Text;

class Program
{
	static void Main(string[] args)
	{
		var factory = new ConnectionFactory() { HostName = "localhost" };

		using (var connection = factory.CreateConnectionAsync())
		using (var channel = connection.Result.CreateChannelAsync())
		{
			channel.Result.QueueDeclareAsync(
				queue: "userQueue",
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);

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