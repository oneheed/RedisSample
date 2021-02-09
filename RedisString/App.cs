using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Core;
using StackExchange.Redis;

namespace RedisString
{
    class App : BaseConnectionMultiplexer
    {
        public App(
            ILoggerFactory factory,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection) : base(factory.CreateLogger<App>(), appLifetime, connection)
        {
        }

        private static Stopwatch sw = new Stopwatch();

        public override async Task ExcuteFunc(CancellationToken cancellationToken)
        {
            var stringKey = "stringKey";
            this._cache.KeyDelete(stringKey);
            this._cache.KeyDelete($"{stringKey}2");
            this._cache.StringSet(stringKey, $"hello {stringKey}");
            this._cache.StringSetRange(stringKey, 6, "test");
            this._cache.StringSetRange($"{stringKey}2", 6, "test");
            this._cache.StringGet($"{stringKey}2");
            var test = this._cache.StringGetLease($"{stringKey}2");
            var test1 = this._cache.StringGetLease($"{stringKey}2");
            //this._cache.KeyExpire("stringKey", TimeSpan.FromSeconds(15));


            //var bactch = this._cache.CreateBatch();
            //var data = Enumerable.Range(1, 10)
            //    .Select(x => new KeyValuePair<RedisKey, RedisValue>($"stringKey{x}", $"stringKey{x}"));

            //var deleteTasks = bactch.KeyDeleteAsync(data.Select(x => x.Key).ToArray());
            ////bactch.Execute();

            //var setTasks = bactch.StringSetAsync(data.ToArray());
            //var expireTasks = data.Select(x => bactch.KeyExpireAsync(x.Key, TimeSpan.FromSeconds(10))).ToList();

            //bactch.Execute();

            //var test1 = await Task.WhenAll(deleteTasks);
            //var test2 = await Task.WhenAll(setTasks);
            //var test3 = await Task.WhenAll(expireTasks);

            //return Task.CompletedTask;
        }
    }
}
