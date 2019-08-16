using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Hyprsoft.Logging.Core
{
    public class ExceptionLogger
    {
        #region Fields

        private readonly object _lock = new object();
        private static ExceptionLogger _instance;

        #endregion

        #region Constructors

        private ExceptionLogger() { }

        #endregion

        #region Properties

        public const string DefaultLogFilenameExtension = "json";
        public const string DefaultLogFilenameDateFormat = "yyyy-MM-dd-hh-mm-ss-fff";

        public static ExceptionLogger Instance { get { return _instance ?? (_instance = new ExceptionLogger()); } }

        public string RootFolder { get; set; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string LogFilenameExtension { get; set; } = DefaultLogFilenameExtension;

        public string LogFilenameDateFormat { get; set; } = DefaultLogFilenameDateFormat;

        #endregion

        #region Methods

        public string Log(Exception ex)
        {
            lock (_lock)
            {
                var logFilename = Path.Combine(RootFolder, $"{DateTime.Now.ToString(LogFilenameDateFormat)}-{ex.GetType().Name}.{LogFilenameExtension}").ToLower();
                using (var writer = File.AppendText(logFilename))
                    writer.Write(JsonConvert.SerializeObject(ex, Formatting.Indented));
                return logFilename;
            }
        }

        #endregion
    }
}
