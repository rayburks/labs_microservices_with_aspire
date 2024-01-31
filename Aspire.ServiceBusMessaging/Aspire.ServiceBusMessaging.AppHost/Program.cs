var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Aspire_ServiceBusMessaging_WeatherForecastApi>("aspire.servicebusmessaging.weatherforecastapi");

builder.AddProject<Projects.Aspire_ServiceBusMessaging_DemoSubscriptonService>("aspire.servicebusmessaging.demosubscriptonservice");

builder.AddProject<Projects.Aspire_ServiceBusMessaging_ProxyApi>("aspire.servicebusmessaging.proxyapi");

builder.AddProject<Projects.Aspire_ServiceBusMessaging_SampleSubscriptionService>("aspire.servicebusmessaging.samplesubscriptionservice");

builder.Build().Run();
