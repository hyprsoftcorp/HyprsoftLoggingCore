using Microsoft.Extensions.Logging;
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

        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public int MaxFileSizeBytes { get; set; } = DefaultMaxFileSizeBytes;

        public int MaxArchiveFileCount { get; set; } = DefaultMaxArchiveFileCount;

        #endregion
    }
}
