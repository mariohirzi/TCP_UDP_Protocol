using Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        int port = context.Configuration.GetValue<int>("ServerSetting:Port");
        services.Configure<ServerOptions>(option => option.Port = port);
        services.AddHostedService<WorkerTCP>();
        services.AddHostedService<UDPWorker>();
    })
    .Build();

await host.RunAsync();
