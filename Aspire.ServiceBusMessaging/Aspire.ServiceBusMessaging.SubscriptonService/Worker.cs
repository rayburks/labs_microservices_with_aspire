using Azure.Messaging.ServiceBus;
using GeneratedCode;
using Refit;

namespace Aspire.ServiceBusMessaging.DemoSubscriptonService;

public class Worker(
    ILogger<Worker> logger,
    ServiceBusClient client,
    IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processor = client.CreateProcessor(
                "demotopic",
                "demosubscription",
                new ServiceBusProcessorOptions());

            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            // start processing
            await processor.StartProcessingAsync();

            logger.LogInformation(
                "Wait for a minute and then press any key to end the processing");
            Console.ReadKey();

            // stop processing
            logger.LogInformation("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            logger.LogInformation("Stopped receiving messages");
        }
    }

    async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        logger.LogInformation("Received from demotopic: {Body} from subscription.", body);

        //ServiceBusMessagingWeatherForecastApi

        var forecastApi = RestService.For<IAspireServiceBusMessagingWeatherForecastApi>(configuration["WeatherForecastApiBaseUrl"]);
        var forecasts = await forecastApi.GetWeatherForecast();

        foreach (var forecast in forecasts)
        {
            logger.LogInformation($"Summary: {forecast.Summary} Date:{forecast.Date} C:{forecast.TemperatureC}");
        }

        // complete the message. messages is deleted from the subscription.
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    Task ErrorHandler(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, args.Exception.Message);
        return Task.CompletedTask;
    }
}
