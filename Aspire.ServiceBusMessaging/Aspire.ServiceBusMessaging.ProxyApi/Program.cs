using Azure.Messaging.ServiceBus;
using GeneratedCode;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBus("ServiceBusConnection");

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddRefitClient<IAspireServiceBusMessagingWeatherForecastApi>()
//    .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["WeatherForecastApiBaseUrl"]));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


//sampletopic

app.MapPost("/notify_sampletopic", static async (ServiceBusClient client, string message) =>
{
    var sender = client.CreateSender("sampletopic");

    // Create a batch
    using ServiceBusMessageBatch messageBatch =
        await sender.CreateMessageBatchAsync();

    if (messageBatch.TryAddMessage(
            new ServiceBusMessage($"Message {message}")) is false)
    {
        // If it's too large for the batch.
        throw new Exception(
            $"The message {message} is too large to fit in the batch.");
    }

    // Use the producer client to send the batch of
    // messages to the Service Bus topic.
    await sender.SendMessagesAsync(messageBatch);

    Console.WriteLine($"A message has been published to the topic.");
});

app.MapPost("/notify_demotopic", static async (ServiceBusClient client, string message) =>
{
    var sender = client.CreateSender("demotopic");

    // Create a batch
    using ServiceBusMessageBatch messageBatch =
        await sender.CreateMessageBatchAsync();

    if (messageBatch.TryAddMessage(
            new ServiceBusMessage($"Message {message}")) is false)
    {
        // If it's too large for the batch.
        throw new Exception(
            $"The message {message} is too large to fit in the batch.");
    }

    // Use the producer client to send the batch of
    // messages to the Service Bus topic.
    await sender.SendMessagesAsync(messageBatch);

    Console.WriteLine($"A message has been published to the topic.");
});


app.MapGet("/weatherforecast", (IConfiguration config) =>
    {
        var forecastApi = RestService.For<IAspireServiceBusMessagingWeatherForecastApi>(config["WeatherForecastApiBaseUrl"]);
        var forecast = forecastApi.GetWeatherForecast().Result.ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();