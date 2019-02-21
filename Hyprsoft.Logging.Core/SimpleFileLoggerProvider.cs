using Microsoft.Extensions.Logging;

namespace Hyprsoft.Logging.Core
{
    public class SimpleFileLoggerProvider : ILoggerProvider
    {
        #region Fields

        private readonly SimpleFileLoggerOptions _settings;

        #endregion

        #region Constructors

        internal SimpleFileLoggerProvider(SimpleFileLoggerOptions settings)
        {
            _settings = settings;
        }

        #endregion

        #region Methods

        public ILogger CreateLogger(string categoryName)
        {
            return new SimpleFileLogger(categoryName, _settings);
        }

        public void Dispose()
        {
        }

        #endregion
    }
}
