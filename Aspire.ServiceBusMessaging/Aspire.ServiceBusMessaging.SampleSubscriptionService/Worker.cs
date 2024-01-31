using Azure.Messaging.ServiceBus;

namespace Aspire.ServiceBusMessaging.SampleSubscriptionService;

public class Worker(
    ILogger<Worker> logger,
    ServiceBusClient client) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var processor = client.CreateProcessor(
                "sampletopic",
                "samplesubscription",
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
        logger.LogInformation("Received from sampletopic: {Body} from subscription.", body);

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
