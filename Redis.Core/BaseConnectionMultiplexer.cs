using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Redis.Core
{
    public abstract class BaseConnectionMultiplexer : BaseHostedService
    {
        protected IDatabase _cache;
        protected IConnectionMultiplexer _connection;

        protected BaseConnectionMultiplexer(
            ILogger logger,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection) : base(logger, appLifetime)
        {
            this._connection = connection;
            this._cache = connection.GetDatabase();
        }
    }
}
