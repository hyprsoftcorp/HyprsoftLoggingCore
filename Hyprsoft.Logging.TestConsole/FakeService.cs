using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Hello World!");
        }
    }
}
