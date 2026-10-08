namespace WebApplication26.HostedService
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        public WorkerService(ILogger<WorkerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WorkerService запущено.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Фонове завдання працює: {time}", DateTime.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
