using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Worker
{
    public class UDPWorker : BackgroundService
    {
        private readonly ILogger<UDPWorker> _logger;
        private readonly ServerOptions _serverSettings;


        private string messageToClients;

        public UDPWorker(ILogger<UDPWorker> logger, IOptions<ServerOptions> options)
        {
            _logger = logger;
            _serverSettings = options.Value;
            messageToClients = "12345";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _serverSettings.Port));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("UDP waiting for client");

                    UdpReceiveResult receivedData = await udpClient.ReceiveAsync(stoppingToken);

                    _logger.LogInformation("Received Data");

                    Task.Run(() => RunUDP(receivedData.RemoteEndPoint), stoppingToken);
                }
                catch
                {
                    _logger.LogWarning("Failure.");
                }
            }
        }

        private void RunUDP(IPEndPoint iPEndPoint)
        {
            Thread.Sleep(1000);

            try
            {
                UdpClient udpClient = new UdpClient();
                udpClient.Send(new byte[] {1,2,3,4,1},5,iPEndPoint);
                _logger.LogInformation("Message send");
            }
            catch
            {
                _logger.LogWarning("Failed to send message!");
                return;
            }


        }
    }
}