using Microsoft.Extensions.Logging;

namespace Benchmark.LoggerTest
{
    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        public void DoAction(string name, int count)
        {
            for (var i = 0; i < count; i++)
            {
                _logger.LogDebug(20, "Doing hard work! {Action}", name);
            }
        }
    }
}