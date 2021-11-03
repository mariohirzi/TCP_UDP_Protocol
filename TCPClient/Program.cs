using System.Net;
using TCPClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context,services) =>
    {
        int port = context.Configuration.GetValue<int>("ServerSetting:Port");
        string address = context.Configuration.GetValue<string>("ServerSetting:IPAddress");
        services.Configure<ServerSettings>(option => option.EndPoint = new IPEndPoint(IPAddress.Parse(address), port));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
