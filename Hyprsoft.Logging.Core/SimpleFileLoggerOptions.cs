using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Hyprsoft.Logging.Core
{
    public class SimpleFileLoggerOptions
    {
        #region Properties

        public const string DefaultLogFilename = "app-log.log";
        public const int DefaultMaxFileSizeBytes = 0;
        public const int DefaultMaxArchiveFileCount = 5;

        public string RootFolder { get; set; } = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public string Filename { get; set; } = DefaultLogFilename;

        public Func<LogLevel, bool> Filter { get; set; } = logLevel => logLevel != LogLevel.None;

        public int MaxFileSizeBytes { get; set; } = DefaultMaxFileSizeBytes;

        public int MaxArchiveFileCount { get; set; } = DefaultMaxArchiveFileCount;

        #endregion
    }
}
