using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Core;
using StackExchange.Redis;

namespace RedisPubSubSample
{
    class App : BaseConnectionMultiplexer
    {
        public App(
            ILoggerFactory factory,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection) : base(factory.CreateLogger<App>(), appLifetime, connection, (int)RedisType.String)
        {
        }

        public override async Task ExcuteFunc(CancellationToken cancellationToken)
        {
            var sub = this._connection.GetSubscriber();
            var readedCount = 0;

            sub.Subscribe("SequentialChannel", (channel, message) =>
            {
                this._logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss.fff} - Received message {message}");

                readedCount++;
            });
        }
    }
}
