using Aspire.ServiceBusMessaging.SampleSubscriptionService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAzureServiceBus("ServiceBusConnection");

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
