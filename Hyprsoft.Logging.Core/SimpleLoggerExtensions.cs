using System.Runtime.CompilerServices;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Hyprsoft.Logging.Tests")]

namespace Hyprsoft.Logging.Core
{
    public static class SimpleLoggerExtensions
    {
        #region Methods

        public static ILoggerFactory AddSimpleFileLogger(this ILoggerFactory loggerFactory, LogLevel minLevel = LogLevel.Information)
        {
            var options = new SimpleFileLoggerOptions();
            return loggerFactory.AddSimpleFileLogger(options, minLevel);
        }

        public static ILoggerFactory AddSimpleFileLogger(this ILoggerFactory loggerFactory, SimpleFileLoggerOptions options, LogLevel minLevel = LogLevel.Information)
        {
            options.Filter = level => level >= minLevel && level != LogLevel.None;
            loggerFactory.AddProvider(new SimpleFileLoggerProvider(options));
            return loggerFactory;
        }

        public static ILoggerFactory AddSimpleFileLogger(this ILoggerFactory loggerFactory, Action<SimpleFileLoggerOptions> options, LogLevel minLevel = LogLevel.Information)
        {
            var o = new SimpleFileLoggerOptions();
            options(o);
            return loggerFactory.AddSimpleFileLogger(o, minLevel);
        }

        public static ILoggingBuilder AddSimpleFileLogger(this ILoggingBuilder builder)
        {
            builder.Services.AddSingleton<ILoggerProvider, SimpleFileLoggerProvider>(services => new SimpleFileLoggerProvider(new SimpleFileLoggerOptions()));
            return builder;
        }

        public static ILoggingBuilder AddSimpleFileLogger(this ILoggingBuilder builder, Action<SimpleFileLoggerOptions> configure)
        {
            var options = new SimpleFileLoggerOptions();
            configure(options);
            builder.Services.AddSingleton<ILoggerProvider, SimpleFileLoggerProvider>(services => new SimpleFileLoggerProvider(options));

            return builder;
        }

        #endregion
    }
}
