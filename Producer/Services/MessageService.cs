using System.Text;
using RabbitMQ.Client;

namespace Producer;
public interface IMessageService 
{
    bool Enqueue(string message);
}

public class MessageService : IMessageService
{
    //TODO: make a correct way to connect
    ConnectionFactory _factory;
    IConnection _connection;
    IModel _channel;

    public MessageService()
    {
        System.Console.WriteLine("about to connect to rabbit");

        _factory = new ConnectionFactory() 
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = _factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare( queue: "hello",
                                durable: false,
                                exclusive: false,
                                autoDelete: false,
                                arguments: null);
    }

    public bool Enqueue(string message)
    {
        var body = Encoding.UTF8.GetBytes($"server processed {message}");
        _channel.BasicPublish( exchange: "",
                                routingKey: "hello",
                                basicProperties: null,
                                body: body);
        System.Console.WriteLine($" * published {message} to rabbit");
        return true;
    }
}