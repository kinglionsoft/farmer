using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace NlogBenchmark
{
    public class Program
    {
        const int N = 10000;
        public static void Main(string[] args)
        {
            var serviceProvider = BuildDi();
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("Test");
            var message = new string('x', 200);
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < N; i++)
            {
                logger.LogDebug(message);
            }
            sw.Stop();
            System.Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ": " + sw.ElapsedTicks);
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });

            NLog.LogManager.LoadConfiguration("nlog.config");

            return serviceProvider;
        }
    }
}
