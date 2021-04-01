using System.Linq;
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
            IConnectionMultiplexer connection,
            int database = 0) : base(logger, appLifetime)
        {
            this._connection = connection;
            this._cache = connection.GetDatabase(database);
        }

        protected bool ResetDatabase()
        {
            var server = this._connection.GetServer(this._connection.GetEndPoints().First());
            return this._cache.KeyDelete(server.Keys(this._cache.Database).ToArray()) > default(long);
        }
    }
}
