using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Redis.Core
{
    public abstract class BaseHostedService : IHostedService
    {
        protected int? _exitCode;

        protected ILogger _logger;
        protected IHostApplicationLifetime _appLifetime;

        protected BaseHostedService(
           ILogger logger,
           IHostApplicationLifetime appLifetime)
        {
            this._logger = logger;
            this._appLifetime = appLifetime;
        }

        public abstract Task ExcuteFunc(CancellationToken cancellationToken);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("APP running at: {time}", DateTimeOffset.Now);

                await ExcuteFunc(cancellationToken);

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
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("App stopped at: {time}", DateTimeOffset.Now);

            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);

            return Task.CompletedTask;
        }
    }
}
