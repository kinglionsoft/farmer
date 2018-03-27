using System;
using System.IO;
using System.Security.Cryptography;
using Benchmark.LoggerTest;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Exporters;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Benchmark
{
    [CoreJob]
    [RPlotExporter, RankColumn]
    public class LoggerBenchmark
    {
        private Runner _runner;
        private string _content;

        [Params(10, 100)]
        public int N;

        [GlobalSetup]
        public void Setup()
        {
            var serviceProvider = BuildDi();
            _runner = serviceProvider.GetService<Runner>();
            _content = new string('x', 200);
        }

        [Benchmark]
        public void Run()
        {
            _runner.DoAction(_content, N);
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            //Runner is the custom class
            services.AddTransient<Runner>();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            // loggerFactory.ConfigureNLog("nlog.config");

            NLog.LogManager.LoadConfiguration(@"E:\GitHub\farmer\Benchmark\nlog.config");

            return serviceProvider;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
             BenchmarkRunner.Run<LoggerBenchmark>();
             System.Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"));
        }
    }
}
