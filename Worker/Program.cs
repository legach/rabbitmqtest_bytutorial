// See https://aka.ms/new-console-template for more information
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

const string Host = "http://producer:8080";

string[] testStrings = new[] {"one", "two", "three", "four", "five"};

System.Console.WriteLine("sleeping to wait for rabbit");
await Task.Delay(10_000);
System.Console.WriteLine("posting message to webapi");

foreach (var item in testStrings)
{
    await PostMessage(item);
}

await Task.Delay(1000);
System.Console.WriteLine("consumin queue now");

var factory = new ConnectionFactory()
{
    HostName = "rabbitmq",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
};

var connection = factory.CreateConnection();
var channel = connection.CreateModel();
channel.QueueDeclare( queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, eventArgs) => 
{
    var body = eventArgs.Body;
    var message = Encoding.UTF8.GetString(body.ToArray());
    System.Console.WriteLine($" * received from rabbit: {message}");
};
channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

static async Task PostMessage(string postData)
{
    var json = JsonConvert.SerializeObject(postData);
    var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
    using HttpClientHandler clientHandler = new HttpClientHandler();
    //Make local https possible
    clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
    using HttpClient client = new HttpClient(clientHandler);
    var result = await client.PostAsync($"{Host}/values", content);
    var resultContent = await result.Content.ReadAsStringAsync();
    Console.WriteLine($"Server returned: {resultContent}. Status: {result.StatusCode}");
}