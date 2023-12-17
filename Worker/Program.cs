// See https://aka.ms/new-console-template for more information
using System.Text;
using Newtonsoft.Json;

const string Host = "https://localhost:7268";

Console.WriteLine("Hello, World!");
await PostMessage("test message");


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
    Console.WriteLine($"Server returned: {resultContent}");
}