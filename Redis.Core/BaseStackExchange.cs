using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Redis.Core
{
    public abstract class BaseStackExchange : BaseHostedService
    {
        protected IDistributedCache _cache;

        protected BaseStackExchange(
            ILogger logger,
            IHostApplicationLifetime appLifetime,
            IDistributedCache cache) : base(logger, appLifetime)
        {
            this._cache = cache;
        }
    }
}
