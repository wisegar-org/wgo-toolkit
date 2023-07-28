using WGOvh.Settings;

namespace WGOvh.Services
{
    public class WorkerService : BackgroundService
    {
		private readonly ILogger<WorkerService> _logger;
        private readonly MeService _apiService;
		private readonly DomainService _domainService;

		public WorkerService(ILogger<WorkerService> logger, IConfiguration configuration)
        {
            _logger = logger;
            var ovhSettings = OvhSettings.GetSettings(configuration);
			_apiService = new MeService(ovhSettings);
            _domainService = new DomainService(logger, configuration);

		}

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
			_logger.LogInformation("WGO Worker started at: {time}", DateTimeOffset.Now);

			while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("WGO Worker running at: {time}", DateTimeOffset.Now);
                try {
					var externalIp = await SettingsService.GetPublicIp();

					if (externalIp == null)
					{
						_logger.LogCritical("Imposible to get network public ip!");
						continue;
					}

					await _domainService.UpdateDomains(externalIp);

				} catch (Exception e) {
					_logger.LogError($"WGO Worker error. {e.Message}", e);
				} 
                  
				await Task.Delay(5000, stoppingToken);
            }
        }
    }
}