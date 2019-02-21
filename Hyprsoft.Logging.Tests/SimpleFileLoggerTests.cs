using Hyprsoft.Logging.Core;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Tests
{
    [TestClass]
    public class SimpleFileLoggerTests
    {
        #region Fields

        private const string CategoryName = "Unit Tests";

        #endregion  

        #region Methods

        [TestCleanup]
        public void Cleanup()
        {
            // Delete all log files.
            foreach (var filename in Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"*{Path.GetExtension(SimpleFileLoggerOptions.DefaultLogFilename)}"))
                System.IO.File.Delete(filename);
        }

        [TestMethod]
        public async Task LogLevelCheckTrace()
        {
            await ValidateLogEntriesAsync(LogLevel.Trace, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Trace" },
                { 3, "Debug" },
                { 5, "Information" },
                { 7, "Warning" },
                { 9, "Error" },
                { 11, "Critical" }
            });
        }

        [TestMethod]
        public async Task LogLevelCheckDebug()
        {
            await ValidateLogEntriesAsync(LogLevel.Debug, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Debug" },
                { 3, "Information" },
                { 5, "Warning" },
                { 7, "Error" },
                { 9, "Critical" }
            });
        }

        [TestMethod]
        public async Task LogLevelCheckInformation()
        {
            await ValidateLogEntriesAsync(LogLevel.Information, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Information" },
                { 3, "Warning" },
                { 5, "Error" },
                { 7, "Critical" }
            });
        }

        [TestMethod]
        public async Task LogLevelCheckWarning()
        {
            await ValidateLogEntriesAsync(LogLevel.Warning, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Warning" },
                { 3, "Error" },
                { 5, "Critical" }
            });
        }

        [TestMethod]
        public async Task LogLevelCheckError()
        {
            await ValidateLogEntriesAsync(LogLevel.Error, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Error" },
                { 3, "Critical" }
            });
        }

        [TestMethod]
        public async Task LogLevelCheckCritical()
        {
            await ValidateLogEntriesAsync(LogLevel.Critical, new Dictionary<int, string> {
                { 0, CategoryName },
                { 1, "Critical" }
            });
        }

        [TestMethod]
        public async Task ArchiveCheck()
        {
            using (var provider = new SimpleFileLoggerProvider(new SimpleFileLoggerOptions
            {
                RootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                LogLevel = LogLevel.Trace,
                MaxFileSizeBytes = 1024
            }))
            {
                var logger = provider.CreateLogger(CategoryName);
                for (var i = 1; i <= 10; i++)
                {
                    for (var j = 1; j <= 15; j++)
                        logger.LogTrace($"{i}-{j}");

                    // It appears as though we can write files too quickly so that sorting them later by LastWriteTime
                    // doesn't work properly.  Based on the docs there is some caching going on.  This seems to fix it.
                    // In real life these log files won't be archived this quickly anyway.
                    // https://docs.microsoft.com/en-us/dotnet/api/system.io.filesysteminfo.lastwritetimeutc?view=netframework-4.7.2
                    await Task.Delay(500);
                }   // file for loop
            }   // using log provider.

            Assert.AreEqual(6, Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"*{Path.GetExtension(SimpleFileLoggerOptions.DefaultLogFilename)}").Length);
        }

        private async Task ValidateLogEntriesAsync(LogLevel logLevel, Dictionary<int, string> truth)
        {
            using (var provider = new SimpleFileLoggerProvider(new SimpleFileLoggerOptions
            {
                RootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                LogLevel = logLevel
            }))
            {
                var logger = provider.CreateLogger(CategoryName);

                logger.LogTrace("Trace");
                logger.LogDebug("Debug");
                logger.LogInformation("Information");
                logger.LogWarning("Warning");
                logger.LogError("Error");
                logger.LogCritical("Critical");
                logger.Log(LogLevel.None, "None");

                var entries = await File.ReadAllLinesAsync(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SimpleFileLoggerOptions.DefaultLogFilename));
                Assert.AreEqual((truth.Count - 1) * 2, entries.Length);
                foreach (var kvp in truth)
                    Assert.IsTrue(entries[kvp.Key].Contains(kvp.Value));
            }   // using log provider.
        }

        #endregion
    }
}
