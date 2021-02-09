using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Redis.Core
{
    public static class HostBuilder
    {
        public static IHostBuilder CreateHostBuilder(string[] args, RedisCreateType type, Func<IServiceCollection, IServiceCollection> fun) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureLogging(logging =>
                //{
                //    logging.SetMinimumLevel(LogLevel.Warning);
                //})
                .ConfigureServices(services =>
                {
                    switch (type)
                    {
                        case RedisCreateType.StackExchange:
                            // Use Stack Exchange
                            services.AddStackExchangeRedisCache(options =>
                            {
                                options.Configuration = "127.0.0.1:6379";
                                //options.InstanceName = "SampleInstance";
                            });

                            break;

                        case RedisCreateType.ConnectionMultiplexer:
                            // User Connection Multiplexer
                            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
                                new ConfigurationOptions
                                {
                                    EndPoints = { { "127.0.0.1", 6379 } },
                                }));

                            break;
                    }

                    fun(services);
                });
    }
}
