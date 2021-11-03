using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace UDPClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServerSettings _serverSettings;

        public Worker(ILogger<Worker> logger, IOptions<ServerSettings> options)
        {
            _logger = logger;
            _serverSettings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            UdpClient client = new UdpClient();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    client.Send(new byte[1], 1,_serverSettings.EndPoint);
                    _logger.LogInformation("Message successful send to the endpoint.");
                }
                catch
                {
                    _logger.LogWarning("Can't send message to the endpoint.");
                    return;
                }

                _logger.LogInformation("Start receiving data");
                try
                {
                    UdpReceiveResult result = await client.ReceiveAsync();

                    string? msg = string.Join(",", result.Buffer);
                    _logger.LogInformation($"Received Message: {msg}");
                }
                catch
                {
                    _logger.LogWarning("Can't receive message to the endpoint.");
                    return;
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}