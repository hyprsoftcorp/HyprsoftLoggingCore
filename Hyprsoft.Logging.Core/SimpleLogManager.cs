using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Core
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error
    }

    public class SimpleLogManager
    {
        #region Fields

        private List<SimpleLogger> _loggers = new List<SimpleLogger>();

        #endregion

        #region Constructors

        public SimpleLogManager()
        {
            _loggers.Add(new SimpleDebugLogger());
        }

        #endregion

        #region Events

        public event EventHandler<SimpleLoggerEventArgs> Logged;

        #endregion

        #region Properties

        public LogLevel LogLevel { get; set; } = LogLevel.Info;

        public ReadOnlyCollection<SimpleLogger> Loggers => _loggers.AsReadOnly();

        #endregion

        #region Methods

        public void AddLogger(SimpleLogger logger)
        {
            if (!_loggers.Contains(logger))
                _loggers.Add(logger);
            else
                throw new InvalidOperationException("Logger has already been added.");
        }

        public void RemoveLogger(SimpleLogger logger)
        {
            if (_loggers.Contains(logger))
                _loggers.Remove(logger);
            else
                throw new InvalidOperationException("Logger has not been added.");
        }

        public void ClearLoggers()
        {
            _loggers.Clear();
        }

        public async Task LogAsync<T>(LogLevel logLevel, string message)
        {
            if (!IsLogLevelValid(logLevel))
                return;

            foreach (var logger in _loggers)
                await logger.LogAsync<T>(logLevel, message);

            OnLogged(new SimpleLoggerEventArgs { LogLevel = logLevel, Message = message });
        }

        public async Task LogAsync<T>(Exception ex, string message)
        {
            if (!IsLogLevelValid(LogLevel.Error))
                return;

            var messageWithStackTrace = $"{message}\n\t{ex}";

            foreach (var logger in _loggers)
                await logger.LogAsync<T>(LogLevel.Error, messageWithStackTrace);

            OnLogged(new SimpleLoggerEventArgs { LogLevel = LogLevel.Error, Message = messageWithStackTrace });
        }

        protected virtual void OnLogged(SimpleLoggerEventArgs args)
        {
            Logged?.Invoke(this, args);
        }

        private bool IsLogLevelValid(LogLevel logLevel)
        {
            return logLevel >= LogLevel;
        }

        #endregion
    }
}
