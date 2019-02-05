using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Core
{
    public class SimpleDebugLogger : SimpleLogger
    {
        #region Properties

        public override Guid Id => Guid.Parse("46B40E8D-DC82-4F85-B30A-F1ED325E38BD");

        #endregion

        #region Methods

        protected override Task OnLogAsync<T>(LogLevel logLevel, string message)
        {
            Trace.WriteLine($"{logLevel.ToString().ToUpper()}:\t{typeof(T).FullName} @ {DateTime.Now}\n\t{message}");
            return Task.CompletedTask;
        }

        #endregion
    }
}
