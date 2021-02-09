using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RedisSample
{
    class StackExchangeApp : IHostedService
    {
        private int? _exitCode;

        private readonly ILogger<StackExchangeApp> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IDistributedCache _cache;

        public StackExchangeApp(
            ILogger<StackExchangeApp> logger,
            IHostApplicationLifetime appLifetime,
            IDistributedCache cache)
        {
            this._logger = logger;
            this._appLifetime = appLifetime;
            this._cache = cache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("App running at: {time}", DateTimeOffset.Now);

            this._cache.SetString("LessSharp:Test:A", "Test1");
            this._cache.Set("LessSharp:Test:B", Encoding.UTF8.GetBytes("value"), new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3000)));
            var test = Encoding.UTF8.GetString(this._cache.Get("LessSharp:Test:A"));

            try
            {
                _logger.LogWarning("App running at: {time}", DateTimeOffset.Now);
                _exitCode = 999;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception!");
                _exitCode = 999;
            }
            finally
            {
                _appLifetime.StopApplication();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("App stopped at: {time}", DateTimeOffset.Now);

            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);

            return Task.CompletedTask;
        }
    }
}
