using System;

namespace Hyprsoft.Logging.Core
{
    public class SimpleLoggerEventArgs : EventArgs
    {
        public LogLevel LogLevel { get; set; }

        public string Message { get; set; }
    }
}
