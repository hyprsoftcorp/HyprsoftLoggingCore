using System;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Hyprsoft.Logging.Core
{
    public class SimpleFileLogger : ILogger
    {
        #region Fields

        private readonly string _category;
        private readonly SimpleFileLoggerOptions _options;
        private readonly object _lock = new object();

        #endregion

        #region Constructors

        internal SimpleFileLogger(string category, SimpleFileLoggerOptions options)
        {
            _category = category;
            _options = options;
        }

        #endregion

        #region Methods

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _options.Filter.Invoke(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            lock (_lock)
            {
                var fullFilename = Path.Combine(_options.RootFolder, _options.Filename);
                var fileInfo = new FileInfo(fullFilename);
                if (_options.MaxFileSizeBytes > 0 && fileInfo.Exists && fileInfo.Length > _options.MaxFileSizeBytes)
                {
                    var files = Directory.GetFiles(_options.RootFolder, $"{Path.GetFileNameWithoutExtension(_options.Filename)}*{Path.GetExtension(_options.Filename)}")
                        .Where(f => String.Compare(f, fullFilename, true) != 0)
                        .Select(f => new FileInfo(f))
                        .OrderBy(f => f.LastWriteTime).ToList();
                    if (_options.MaxArchiveFileCount > 0 && files.Count >= _options.MaxArchiveFileCount)
                        files[0].Delete();
                    fileInfo.MoveTo(Path.Combine(_options.RootFolder, $"{Path.GetFileNameWithoutExtension(_options.Filename)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(_options.Filename)}"));
                    fileInfo = new FileInfo(Path.Combine(_options.RootFolder, _options.Filename));
                }
                using (var writer = fileInfo.AppendText())
                {
                    if (eventId == 0)
                        writer.WriteLine($"{ToShortLogLevel(logLevel)}: {_category} @ {DateTime.Now.ToString("g")}\n\t{formatter(state, exception)}");
                    else
                        writer.WriteLine($"{ToShortLogLevel(logLevel)}: {eventId} {_category} @ {DateTime.Now.ToString("g")}\n\t{formatter(state, exception)}");
                }   // using writer
            }   // lock
        }

        private string ToShortLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    return "CRIT";

                case LogLevel.Debug:
                    return "DBUG";

                case LogLevel.Error:
                    return "FAIL";

                case LogLevel.Information:
                    return "INFO";

                case LogLevel.Trace:
                    return "TRCE";

                case LogLevel.Warning:
                    return "WARN";

                default:
                    return "INFO";
            }
        }

        #endregion
    }
}
