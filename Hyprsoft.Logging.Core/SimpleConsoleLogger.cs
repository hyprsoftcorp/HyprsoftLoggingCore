using System;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Core
{
    public class SimpleConsoleLogger : SimpleLogger
    {
        #region Properties

        public override Guid Id => Guid.Parse("43194F62-7464-44F5-9250-0410DBDA6122");

        #endregion

        #region Methods

        protected override Task OnLogAsync<T>(LogLevel logLevel, string message)
        {
            Console.WriteLine($"{logLevel.ToString().ToUpper()}:\t{typeof(T).FullName} @ {DateTime.Now}\n\t{message}");
            return Task.CompletedTask;
        }

        #endregion
    }
}
