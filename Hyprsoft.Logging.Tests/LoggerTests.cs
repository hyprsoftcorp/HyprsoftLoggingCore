using Hyprsoft.Logging.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Tests
{
    [TestClass]
    public class LoggerTests
    {
        #region Fields

        private string _logFilename;
        private SimpleLogManager _logger;

        #endregion

        #region Methods

        [TestInitialize]
        public void Initialize()
        {
            _logFilename = Path.Combine(Directory.GetCurrentDirectory(), SimpleFileLogger.DefaultLogFilename);
            _logger = new SimpleLogManager();
        }

        [TestMethod]
        public void LogManagerDefaults()
        {
            Assert.AreEqual(LogLevel.Info, _logger.LogLevel);
            Assert.AreEqual(1, _logger.Loggers.Count);
            Assert.AreEqual(typeof(SimpleDebugLogger), _logger.Loggers.First().GetType());
        }

        [TestMethod]
        public void AddRemoveClearLoggers()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _logger.AddLogger(_logger.Loggers.First()));

            var consoleLogger = new SimpleConsoleLogger();
            _logger.AddLogger(consoleLogger);
            Assert.AreEqual(2, _logger.Loggers.Count);

            _logger.RemoveLogger(consoleLogger);
            Assert.AreEqual(1, _logger.Loggers.Count);

            Assert.ThrowsException<InvalidOperationException>(() => _logger.RemoveLogger(consoleLogger));

            _logger.ClearLoggers();
            Assert.AreEqual(0, _logger.Loggers.Count);
        }

        [TestMethod]
        public async Task LogManagerCheck()
        {
            SimpleLoggerEventArgs eventArgs = null;
            _logger.Logged += (s, e) => eventArgs = e;

            _logger.LogLevel = LogLevel.Info;

            await _logger.LogAsync<LoggerTests>(LogLevel.Info, "Info");
            Assert.AreEqual(LogLevel.Info, eventArgs.LogLevel);
            Assert.AreEqual("Info", eventArgs.Message);

            try
            {
                throw new InvalidOperationException("Oops something bad happened.");
            }
            catch (Exception ex)
            {
                await _logger.LogAsync<LoggerTests>(ex, "Error");
                Assert.AreEqual($"Error\n\t{ex.ToString()}", eventArgs.Message);
                Assert.AreEqual(LogLevel.Error, eventArgs.LogLevel);
            }
        }

        [TestMethod]
        public async Task LogLevelsCheck()
        {
            var logged = false;
            _logger.Logged += (s, e) => logged = true;

            _logger.LogLevel = LogLevel.Error;
            await _logger.LogAsync<LoggerTests>(LogLevel.Trace, "Trace");
            await _logger.LogAsync<LoggerTests>(LogLevel.Debug, "Debug");
            await _logger.LogAsync<LoggerTests>(LogLevel.Info, "Info");
            await _logger.LogAsync<LoggerTests>(LogLevel.Warn, "Warn");
            Assert.IsFalse(logged);

            await _logger.LogAsync<LoggerTests>(LogLevel.Error, "Error");
            Assert.IsTrue(logged);

            logged = false;
            _logger.LogLevel = LogLevel.Warn;
            await _logger.LogAsync<LoggerTests>(LogLevel.Trace, "Trace");
            await _logger.LogAsync<LoggerTests>(LogLevel.Debug, "Debug");
            await _logger.LogAsync<LoggerTests>(LogLevel.Info, "Info");
            Assert.IsFalse(logged);

            await _logger.LogAsync<LoggerTests>(LogLevel.Warn, "Warn");
            Assert.IsTrue(logged);

            logged = false;
            await _logger.LogAsync<LoggerTests>(LogLevel.Error, "Error");
            Assert.IsTrue(logged);
        }

        [TestMethod]
        public void FileLoggerDefaults()
        {
            var logger = new SimpleFileLogger();

            Assert.AreEqual(_logFilename, logger.Filename);
            Assert.AreEqual(SimpleFileLogger.DefaultMaxFileSizeBytes, logger.MaxFileSizeBytes);
            Assert.AreEqual(SimpleFileLogger.DefaultMaxArchiveFileCount, logger.MaxArchiveFileCount);

            var fakePath = @"c:\temp\my.log";
            logger = new SimpleFileLogger(fakePath) { MaxArchiveFileCount = 1, MaxFileSizeBytes = 1 };
            Assert.AreEqual(fakePath, logger.Filename);
            Assert.AreEqual(1, logger.MaxArchiveFileCount);
            Assert.AreEqual(1, logger.MaxFileSizeBytes);
        }

        [TestMethod]
        public async Task FileLoggerArchiveCheck()
        {
            Cleanup();
            try
            {
                _logger.LogLevel = LogLevel.Trace;
                _logger.ClearLoggers();

                _logger.AddLogger(new SimpleDebugLogger());
                var logger = new SimpleFileLogger { MaxFileSizeBytes = 1024 };
                _logger.AddLogger(logger);

                for (var i = 1; i <= 10; i++)
                {
                    for (var j = 1; j <= 15; j++)
                        await _logger.LogAsync<LoggerTests>(LogLevel.Trace, $"{i}-{j}");

                    // It appears as though we can write files too quickly so that sorting them later by LastWriteTime
                    // doesn't work properly.  Based on the docs there is some cahcing going on.  This seems to fix it.
                    // In real life these log files won't be archived this quickly anyway.
                    // https://docs.microsoft.com/en-us/dotnet/api/system.io.filesysteminfo.lastwritetimeutc?view=netframework-4.7.2
                    await Task.Delay(1000);
                }   // file for loop

                Assert.AreEqual(6, Directory.GetFiles(Path.GetDirectoryName(_logFilename), $"*{Path.GetExtension(_logFilename)}").Length);
            }
            finally
            {
                Cleanup();
            }
        }

        private void Cleanup()
        {
            // Delete all log files.
            foreach (var filename in Directory.GetFiles(Path.GetDirectoryName(_logFilename), $"*{Path.GetExtension(_logFilename)}"))
                System.IO.File.Delete(filename);
        }


        #endregion
    }
}
