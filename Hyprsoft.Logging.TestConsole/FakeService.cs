using Microsoft.Extensions.Logging;
using System;

namespace Hyprsoft.Logging.TestConsole
{
    public class FakeService
    {
        private readonly ILogger<FakeService> _logger;

        public FakeService(ILogger<FakeService> logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.LogCritical("Critical");
            _logger.LogDebug("Debug");
            try
            {
                throw new InvalidOperationException("Uh oh...something bad happened.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
            }
            _logger.LogInformation("Information");
            _logger.LogTrace("Trace");
            _logger.LogWarning("Waning");
        }
    }
}
