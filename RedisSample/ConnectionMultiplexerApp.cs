using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedisSample
{
    class ConnectionMultiplexerApp : IHostedService
    {
        private int? _exitCode;

        private readonly ILogger<StackExchangeApp> _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IDatabase _cache;

        public ConnectionMultiplexerApp(
            ILogger<StackExchangeApp> logger,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection)
        {
            this._logger = logger;
            this._appLifetime = appLifetime;
            this._cache = connection.GetDatabase();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("App running at: {time}", DateTimeOffset.Now);

            //var hashFields = Enumerable.Range(1, 100).Select(x => new HashEntry(x, x)).ToArray();
            //this._cache.HashSet("LessSharp:Test", hashFields);
            var hashSerach = this._cache.HashScan("LessSharp:Test");
            var test = hashSerach.Take(20).ToList();
            //var test = this._cache.HashGetAll("LessSharp:Test");
            //var testfirst = test.FirstOrDefault(x => x.Name == "A");

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

        /// <summary>
        /// 設置緩存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _cache.StringSet(key, JsonSerializer.Serialize(value), expiry, when, flags);
        }
    }
}
