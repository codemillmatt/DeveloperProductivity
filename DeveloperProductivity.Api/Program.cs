using Azure.Storage.Queues;
using DeveloperProductivity.Models;
using Microsoft.Extensions.Azure;
using System.Text.Json;
using Azure.Storage.Queues.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["storage-connection-string:blob"], preferMsi: true);
    clientBuilder.AddQueueServiceClient(builder.Configuration["storage-connection-string:queue"], preferMsi: true);
});

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
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/currentconditions", () =>
{
    var conditions = new WeatherForecast
    (
        DateTime.Now,
        Random.Shared.Next(-20,55),
        summaries[Random.Shared.Next(summaries.Length)]
    );

    return conditions;
}).WithName("GetCurrentConditions");

app.MapPost("/reportincorrectconditions", async (IncorrectConditionsReport report) =>
{
    var queueServiceClient = app.Services.GetService<QueueServiceClient>();
    var queueClient = queueServiceClient.GetQueueClient("incorrect-conditions-queue");

    await queueClient.CreateIfNotExistsAsync();

    await queueClient.SendMessageAsync(JsonSerializer.Serialize(report).ToString());

}).WithName("ReportIncorrectConditions");

app.Run();
