using System.Net;
using UDPClient;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context,services) =>
    {
        int port = context.Configuration.GetValue<int>("ServerSettings:Port");
        string ipAddress = context.Configuration.GetValue<string>("ServerSettings:IpAddress");

        services.Configure<ServerSettings>(option => option.EndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
