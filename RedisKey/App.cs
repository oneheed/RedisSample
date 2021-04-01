using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Core;
using StackExchange.Redis;

namespace RedisKeySample
{
    class App : BaseConnectionMultiplexer
    {
        public App(
            ILoggerFactory factory,
            IHostApplicationLifetime appLifetime,
            IConnectionMultiplexer connection) : base(factory.CreateLogger<App>(), appLifetime, connection)
        {
        }

        public override async Task ExcuteFunc(CancellationToken cancellationToken)
        {
            _ = base.ResetDatabase();

            // 1. Delete Key (一筆, 多筆)
            var deleteKey = "deleteKey";
            ExcuteRedis(() =>
            {
                this._cache.StringSet(deleteKey, deleteKey);
                return this._cache.KeyDelete(deleteKey);
            }, "KeyDelete result: {result}");

            ExcuteRedis(() =>
            {
                this._cache.StringSet($"{deleteKey}1", $"{deleteKey}1");
                this._cache.StringSet($"{deleteKey}2", $"{deleteKey}2");
                return this._cache.KeyDelete(new RedisKey[] { $"{deleteKey}1", $"{deleteKey}2", $"{deleteKey}3" });
            }, "KeyDelete count: {result}");

            // 2. Dump Key
            var dump = ExcuteRedis(() =>
            {
                var dumpKey = "dumpKey";
                this._cache.StringSet(dumpKey, dumpKey);
                return this._cache.KeyDump(dumpKey);
            }, "KeyDump result: {result}");

            // 3. RESTORE Key
            var restoreKey = "RestoreKey";
            ExcuteRedis(() => { this._cache.KeyRestore(restoreKey, dump); });

            // 4. EXISTS key
            ExcuteRedis(() => { return this._cache.KeyExists(restoreKey); }, "Check restore key exists: {result}");

            // 5. EXPIRE key
            var expireKey = "ExpireKey";
            ExcuteRedis(() =>
            {
                this._cache.StringSet($"{expireKey}1", $"{expireKey}1");
                return this._cache.KeyExpire($"{expireKey}1", DateTime.UtcNow.AddSeconds(60));
            }, "Expire by date time: {result}");

            ExcuteRedis(() =>
            {
                this._cache.StringSet($"{expireKey}2", $"{expireKey}2");
                return this._cache.KeyExpire($"{expireKey}2", new TimeSpan(0, 0, 60));
            }, "Expire by Time Span: {result}");

            // 6. Move Key
            ExcuteRedis(() =>
            {
                this._cache.StringSet("MoveKey", "MoveKey");
                return this._cache.KeyMove("MoveKey", 15);
            }, "Key move: {result}");

            // 7. PERSIST key
            ExcuteRedis(() => { return this._cache.KeyPersist($"{expireKey}1"); }, "ExpireKey1 persist: {result}");
            ExcuteRedis(() => { return this._cache.KeyPersist($"{expireKey}2"); }, "ExpireKey2 persist: {result}");

            // 8. RANDOMKEY
            ExcuteRedis(() => { return this._cache.KeyRandom(); }, "Random key 1: {result}");
            ExcuteRedis(() => { return this._cache.KeyRandom(); }, "Random key 2: {result}");

            // 9. RENAME key newkey
            ExcuteRedis(() =>
            {
                this._cache.StringSet("oldKey", "oldKey");
                return this._cache.KeyRename("oldKey", "newKey");
            }, "renameKey: {result}");

            // 9. TTL key
            ExcuteRedis(() =>
            {
                this._cache.StringSet("TTLKwy", "TTLKwy", new TimeSpan(0, 0, 60));
                return this._cache.KeyTimeToLive("TTLKwy");
            }, "Time To Live: {result}");

            // 10. TYPE key
            ExcuteRedis(() =>
            {
                this._cache.StringSet("TYPEKwy", "TYPEKwy");
                return this._cache.KeyType("TYPEKwy");
            }, "Redis key type: {result}");
        }

        private TResult ExcuteRedis<TResult>(Func<TResult> fun, string logTemplate = "")
        {
            var result = fun();
            this._logger.LogInformation(logTemplate, result);

            return result;
        }

        private void ExcuteRedis(Action fun)
        {
            fun();
        }
    }
}
