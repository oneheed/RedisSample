using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Redis.Core;
using StackExchange.Redis;

namespace RedisBatch
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
            CleaUpData();

            // initial Data ex : 000001, 000002, 000003..etc
            var data = Enumerable.Range(0, 1000).Select(x => x.ToString().PadLeft(6, '0'));

            StopwatchEvent(InsertBy_ListRightPush, data);
            await StopwatchEventAsync(InsertBy_CreateBatchAsync, data);
            StopwatchEvent(InsertBy_LuaScript, data);
            StopwatchEvent(InsertBy_FireAndForget, data);

            //return Task.CompletedTask;
        }

        private void CleaUpData()
        {
            base._cache.KeyDelete(nameof(InsertBy_ListRightPush));
            base._cache.KeyDelete(nameof(InsertBy_CreateBatchAsync));
            base._cache.KeyDelete(nameof(InsertBy_LuaScript));
            base._cache.KeyDelete(nameof(InsertBy_FireAndForget));
        }

        private void StopwatchEvent(Action<IEnumerable<string>> func, IEnumerable<string> input)
        {
            sw.Reset();
            sw.Start();

            func(input);

            sw.Stop();
        }

        private async Task StopwatchEventAsync(Func<IEnumerable<string>, Task> func, IEnumerable<string> input)
        {
            sw.Reset();
            sw.Start();

            await func(input);

            sw.Stop();
        }

        private void InsertBy_ListRightPush(IEnumerable<string> input)
        {
            var name = MethodBase.GetCurrentMethod().Name;

            foreach (var entity in input)
            {
                base._cache.ListRightPush(name, entity);
            }

            base._logger.LogWarning($"{name} total cost : {sw.ElapsedMilliseconds}");
        }

        private async Task InsertBy_CreateBatchAsync(IEnumerable<string> input)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            var batch = base._cache.CreateBatch();

            var tasks = input.Select(item => batch.ListRightPushAsync(name, item))
                .ToList();

            batch.Execute();

            await Task.WhenAll(tasks);

            base._logger.LogWarning($"{name} total cost : {sw.ElapsedMilliseconds}");
        }

        private void InsertBy_LuaScript(IEnumerable<string> input)
        {
            var name = MethodBase.GetCurrentMethod().Name;
            var sb = new StringBuilder();

            foreach (var item in input)
            {
                sb.AppendLine($"redis.call('rpush', 'InsertBy_LuaScript', '{item}')");
            }

            var prepared = LuaScript.Prepare(sb.ToString());
            base._cache.ScriptEvaluate(prepared);

            base._logger.LogWarning($"{name} total cost : {sw.ElapsedMilliseconds}");
        }

        private void InsertBy_FireAndForget(IEnumerable<string> input)
        {
            var name = MethodBase.GetCurrentMethod().Name;

            foreach (var entity in input)
            {
                base._cache.ListRightPush(name, entity, flags: CommandFlags.FireAndForget);
            }

            base._logger.LogWarning($"{name} total cost : {sw.ElapsedMilliseconds}");
        }
    }
}
