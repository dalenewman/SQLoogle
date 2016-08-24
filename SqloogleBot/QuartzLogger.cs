using System;
using Common.Logging.Simple;
using NLog;
using LogLevel = Common.Logging.LogLevel;

namespace SqloogleBot {
    public class QuartzLogger : AbstractSimpleLogger {
        private readonly Logger _logger;

        public QuartzLogger(LogLevel level, bool showLevel, bool showDateTime, bool showLogName, string dateTimeFormat = "o")
            : base("SQLoogleBot", level, showLevel, showDateTime, showLogName, dateTimeFormat) {
            _logger = LogManager.GetLogger("SQLoogleBot");
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception) {
            switch (level) {
                case LogLevel.All:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Error:
                    if (exception == null) {
                        _logger.Error(message.ToString());
                    } else {
                        _logger.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Fatal:
                    if (exception == null) {
                        _logger.Error(message.ToString());
                    } else {
                        _logger.Error(exception, message.ToString());
                    }
                    break;
                case LogLevel.Off:
                    break;
                case LogLevel.Trace:
                    _logger.Debug(message.ToString);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message.ToString());
                    break;
                case LogLevel.Info:
                    _logger.Info(message.ToString());
                    break;
                default:
                    _logger.Info(message.ToString());
                    break;
            }
        }
    }
}
