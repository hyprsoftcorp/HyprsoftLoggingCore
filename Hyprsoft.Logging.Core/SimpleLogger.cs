using System;
using System.Threading.Tasks;

namespace Hyprsoft.Logging.Core
{
    public abstract class SimpleLogger : IEquatable<SimpleLogger>
    {
        #region Properties

        public abstract Guid Id { get; }

        #endregion

        #region Methods

        internal Task LogAsync<T>(LogLevel logLevel, string message)
        {
            return OnLogAsync<T>(logLevel, message);
        }

        internal Task LogAsync<T>(Exception ex)
        {
            return OnLogAsync<T>(LogLevel.Error, ex.ToString());
        }

        protected abstract Task OnLogAsync<T>(LogLevel logLevel, string message);

        #endregion

        #region IEquatable

        public bool Equals(SimpleLogger other)
        {
            if (other == null)
                return false;

            return Id == other.Id;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            var item = obj as SimpleLogger;
            return item == null ? false : Equals(item);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(SimpleLogger item1, SimpleLogger item2)
        {
            if (((object)item1) == null || ((object)item2) == null)
                return Object.Equals(item1, item2);

            return item1.Equals(item2);
        }

        public static bool operator !=(SimpleLogger item1, SimpleLogger item2)
        {
            if (((object)item1) == null || ((object)item2) == null)
                return !Object.Equals(item1, item2);

            return !(item1.Equals(item2));
        }

        #endregion
    }
}
