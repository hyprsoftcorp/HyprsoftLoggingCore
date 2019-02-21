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
        private SimpleFileLoggerOptions _settings;
        private readonly object _lock = new object();

        #endregion

        #region Constructors

        internal SimpleFileLogger(string category, SimpleFileLoggerOptions settings)
        {
            _category = category;
            _settings = settings;
        }

        #endregion

        #region Methods

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _settings.LogLevel && logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            lock (_lock)
            {
                var fullFilename = Path.Combine(_settings.RootFolder, _settings.Filename);
                var fileInfo = new FileInfo(fullFilename);
                if (_settings.MaxFileSizeBytes > 0 && fileInfo.Exists && fileInfo.Length > _settings.MaxFileSizeBytes)
                {
                    var files = Directory.GetFiles(_settings.RootFolder, $"{Path.GetFileNameWithoutExtension(_settings.Filename)}*{Path.GetExtension(_settings.Filename)}")
                        .Where(f => String.Compare(f, fullFilename, true) != 0)
                        .Select(f => new FileInfo(f))
                        .OrderBy(f => f.LastWriteTime).ToList();
                    if (_settings.MaxArchiveFileCount > 0 && files.Count >= _settings.MaxArchiveFileCount)
                        files[0].Delete();
                    fileInfo.MoveTo(Path.Combine(_settings.RootFolder, $"{Path.GetFileNameWithoutExtension(_settings.Filename)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(_settings.Filename)}"));
                    fileInfo = new FileInfo(Path.Combine(_settings.RootFolder, _settings.Filename));
                }
                using (var writer = fileInfo.AppendText())
                {
                    if (eventId == 0)
                        writer.WriteLine($"{logLevel.ToString().Substring(0, 4).ToUpper()}: {_category} @ {DateTime.Now}\n\t{formatter(state, exception)}");
                    else
                        writer.WriteLine($"{logLevel.ToString().Substring(0, 4).ToUpper()}: {eventId} {_category} @ {DateTime.Now}\n\t{formatter(state, exception)}");
                }   // using writer
            }   // lock
        }

        #endregion
    }
}
