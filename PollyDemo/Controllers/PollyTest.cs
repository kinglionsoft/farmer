using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollyDemo.Controllers
{
    public static class PollyTest
    {

        public static async Task Test()
        {
            Polly.Retry.RetryPolicy<int> politicaWaitAndRetry = Polly.Policy
                .HandleResult<int>(i => true)
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(7)
                }, (e, t, i, c) => ReportPollyError(e, t, i, c));

            await politicaWaitAndRetry.ExecuteAsync(() =>
                {
                    Logger.LogInformation("test running");
                    return Task.FromResult(1);
                });
        }

        public static ILogger<HomeController> Logger { get; internal set; }

        public static void ReportPollyError(DelegateResult<int> e, TimeSpan tiempo, int intento, Polly.Context contexto)
        {
            Logger.LogInformation($"Polly重试失败:  重试次数: {intento:00} (调用时长: {tiempo.TotalMinutes} min)\t执行时间: {DateTime.Now}");
        }
    }
}