using System;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Core
{
    public class SimpleFileLogger : SimpleLogger
    {
        #region Fields

        private object _lock = new object();

        #endregion

        #region Constructors

        public SimpleFileLogger()
        {
        }

        public SimpleFileLogger(string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException(nameof(filename));

            Filename = filename;
        }

        #endregion

        #region Properties

        public const string DefaultLogFilename = "app-log.log";
        public const int DefaultMaxFileSizeBytes = 0;
        public const int DefaultMaxArchiveFileCount = 5;

        public override Guid Id => Guid.Parse("4B7681EE-F062-4AC4-9867-CBCB944D23CB");

        public string Filename { get; private set; } = Path.Combine(Directory.GetCurrentDirectory(), SimpleFileLogger.DefaultLogFilename);

        public int MaxFileSizeBytes { get; set; } = DefaultMaxFileSizeBytes;

        public int MaxArchiveFileCount { get; set; } = DefaultMaxArchiveFileCount;

        #endregion

        #region Methods

        protected override Task OnLogAsync<T>(LogLevel logLevel, string message)
        {
            lock (_lock)
            {
                var fileInfo = new FileInfo(Filename);
                if (MaxFileSizeBytes > 0 && fileInfo.Exists && fileInfo.Length > MaxFileSizeBytes)
                {
                    var files = Directory.GetFiles(Path.GetDirectoryName(Filename), $"{Path.GetFileNameWithoutExtension(Filename)}*{Path.GetExtension(Filename)}")
                        .Where(f => String.Compare(f, Filename, true) != 0)
                        .Select(f => new FileInfo(f))
                        .OrderBy(f => f.LastWriteTime).ToList();
                    if (MaxArchiveFileCount > 0 && files.Count >= MaxArchiveFileCount)
                        files[0].Delete();
                    fileInfo.MoveTo(Path.Combine(Path.GetDirectoryName(Filename), $"{Path.GetFileNameWithoutExtension(Filename)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(Filename)}"));
                    fileInfo = new FileInfo(Filename);
                }
                using (var writer = fileInfo.AppendText())
                    writer.WriteLine($"{logLevel.ToString().ToUpper()}:\t{typeof(T).FullName} @ {DateTime.Now}\n\t{message}");
            }   // lock

            return Task.CompletedTask;
        }

        #endregion
    }
}
