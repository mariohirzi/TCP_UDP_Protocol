using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Worker
{
    public class WorkerTCP : BackgroundService
    {
        private readonly ILogger<WorkerTCP> _logger;
        private readonly ServerOptions _serverSettings;

        private List<TcpClient> listOfAllTcpClients;

        private string messageToClients;

        public WorkerTCP(ILogger<WorkerTCP> logger, IOptions<ServerOptions> options)
        {
            _logger = logger;
            _serverSettings = options.Value;
            listOfAllTcpClients = new List<TcpClient>();
            messageToClients = "12345";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TcpWorker starting at: {time}", DateTimeOffset.Now);

            TcpListener listener = new TcpListener(IPAddress.Any, _serverSettings.Port);

            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Tcp listener");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Tcp waiting for client");
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                listOfAllTcpClients.Add(tcpClient);
                Task.Run(() => RunTcpClient(tcpClient, stoppingToken), stoppingToken);
            }
        }

        private void RunTcpClient(TcpClient client, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Run new Tcp Client");

            try
            {
                NetworkStream networkStream = client.GetStream();

                while (!cancellationToken.IsCancellationRequested && client.Connected && networkStream.CanWrite)
                {
                    _logger.LogInformation("Writting message to client" + client.ToString());
                    networkStream.Write(new byte[] { 1, 2, 3, 4, 1 });
                    Thread.Sleep(5000);
                }
            }catch
            {
                _logger.LogWarning("The TCP client terminated the connection abruptly.");
            }
        }
    }
}