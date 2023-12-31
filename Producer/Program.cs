using Microsoft.AspNetCore.Mvc;
using Producer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMessageService, MessageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGroup("/values")
.MapValuesApi()
.WithTags("Values Api")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Payload() {}


public static class RouteBuilderExtension
{
    public static RouteGroupBuilder MapValuesApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", () =>
        {
            return new[] { "value1", "value2" };
        });

        group.MapGet("/{id:int}", (int id) =>
        {
            return "value";
        });

        group.MapPost("/", ([FromBody]string payload, IMessageService messageService) => 
        {
            System.Console.WriteLine($"received a Post: {payload}");
            messageService.Enqueue(payload);
            return Results.Ok();
        });

        return group;
    }
}