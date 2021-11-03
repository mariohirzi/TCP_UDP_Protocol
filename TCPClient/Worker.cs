using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace TCPClient
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
            TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(_serverSettings.EndPoint);
            NetworkStream networkStream= tcpClient.GetStream();
            byte[] buffer = new byte[5];

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await networkStream.ReadAsync(buffer,0, 5);
                string msg = string.Join(",", buffer);
                _logger.LogInformation($"Read {msg} from stream");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}