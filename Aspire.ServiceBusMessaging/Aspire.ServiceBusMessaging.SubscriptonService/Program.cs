using Aspire.ServiceBusMessaging.DemoSubscriptonService;

var builder = Host.CreateApplicationBuilder(args);
builder.AddAzureServiceBus("ServiceBusConnection");

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
