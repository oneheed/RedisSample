using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace RedisSample
{
    static class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).RunConsoleAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureLogging(logging =>
                //{
                //    logging.SetMinimumLevel(LogLevel.Warning);
                //})
                .ConfigureServices(services =>
                {
                    //// Use Stack Exchange
                    //services.AddStackExchangeRedisCache(options =>
                    //{
                    //    options.Configuration = "127.0.0.1:6379";
                    //    //options.InstanceName = "SampleInstance";
                    //});

                    //services.AddHostedService<StackExchangeApp>();

                    // User Connection Multiplexer
                    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(
                        new ConfigurationOptions
                        {
                            EndPoints = { { "127.0.0.1", 6379 } },
                        }));

                    services.AddHostedService<ConnectionMultiplexerApp>();
                });
    }
}
