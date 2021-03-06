﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Redis.Core;
using RedisBatch;

namespace BatchExample
{
    static class Program
    {
        private static IServiceCollection AddHostedService(IServiceCollection services)
            => services.AddHostedService<App>();

        static void Main(string[] args)
        {
            Redis.Core.HostBuilder.CreateHostBuilder(args, CreateType.ConnectionMultiplexer, AddHostedService)
                .RunConsoleAsync();
        }
    }
}
