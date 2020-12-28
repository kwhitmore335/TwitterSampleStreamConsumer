using System;
using System.IO;
using System.Threading.Tasks;
using JHAExercise.Clients;
using JHAExercise.Models;
using JHAExercise.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace JHAExercise
{
    class Program
    {
        const int REPORT_DELAY_MS = 6000;

        static async Task Main()
        {
            var serviceProvider = Startup();

            var input = serviceProvider.GetService<InputProcessor>();
            var output = serviceProvider.GetService<OutputProcessor>();

            var consumeTask = input.ConsumeTweets();

            while (!consumeTask.IsCompleted)
            {                
                await Task.Delay(REPORT_DELAY_MS);
                output.ReportOut();
            }

            serviceProvider.Dispose();
            Console.ReadLine();
        }

        static ServiceProvider Startup()
        {
            var serviceProvider = new ServiceCollection()
            .AddLogging(opt => opt.AddConsole())
            .AddDistributedMemoryCache()
            .AddSingleton(new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build())
            .AddSingleton(new SynchronizedCollection<Tweet>())
            .AddSingleton<ITwitterRestClient, TwitterRestClient>()
            .AddTransient<ITweetRepository, TweetRepository>()
            .AddTransient<OutputProcessor>()
            .AddTransient<InputProcessor>()
            .BuildServiceProvider();

            return serviceProvider;
        }
    }
}
