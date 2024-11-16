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
			string exchangeName = "chat_exchange";
			channel.Result.ExchangeDeclareAsync(
				exchange: exchangeName,
				type: "direct");

			Console.WriteLine("Enter your username:");
			string username = Console.ReadLine();

			Console.WriteLine("Enter recipient's username:");
			string recipient = Console.ReadLine();

			while (true)
			{
				Console.Write("Message: ");
				string message = Console.ReadLine();

				var body = Encoding.UTF8.GetBytes($"{username}: {message}");
				channel.Result.BasicPublishAsync(
					exchange: exchangeName,
					routingKey: recipient,
					body: body);
				Console.WriteLine($"[Sent] {message}");
			}
		}
	}
}