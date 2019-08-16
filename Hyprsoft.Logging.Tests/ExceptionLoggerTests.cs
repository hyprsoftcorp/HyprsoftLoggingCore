using Hyprsoft.Logging.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Hyprsoft.Logging.Tests
{
    [TestClass]
    public class ExceptionLoggerTests
    {
        [TestMethod]
        public void Success()
        {
            string logFilename;
            try
            {
                throw new InvalidOperationException("Oops");
            }
            catch (Exception ex)
            {
                logFilename = ExceptionLogger.Instance.Log(ex);
            }

            Assert.IsTrue(File.Exists(logFilename));
            File.Delete(logFilename);
        }
    }
}
